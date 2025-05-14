using System.Security.Claims;
using CleanIAM.Identity.Api.ViewModels.Signin;
using CleanIAM.Identity.Application.Interfaces;
using CleanIAM.Identity.Application.Queries.Users;
using CleanIAM.Identity.Core.Events;
using CleanIAM.Identity.Core.Users;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using CleanIAM.SharedKernel.Application.Interfaces;
using Wolverine;

namespace CleanIAM.Identity.Api.Controllers;

/// <summary>
/// Controller for handling signin requests.
/// </summary>
[Route("/")]
public class SigninController(
    ISigninRequestService signinRequestService,
    IIdentityBuilderService identityBuilderService,
    IMessageBus bus,
    IAppConfiguration appConfiguration,
    IPasswordHasher passwordHasher,
    ILogger<SigninController> logger) : Controller
{
    /// <summary>
    /// Check on what step is user in signin and eather redirect
    /// him to correct step or show the signin form.
    /// </summary>
    /// <param name="request">Id of the signin flow request</param>
    /// <param name="error">Optional error that occured in some step of the auth flow</param>
    /// <returns></returns>
    [HttpGet("signin")]
    public async Task<IActionResult> Signin([FromQuery] Guid request, [FromQuery] string? error)
    {
        ViewData["Error"] = error;
        ViewData["Request"] = request;

        // Redirect to management portal signin (to get the OIDC flow values)
        var signinRequest = await signinRequestService.GetAsync(request);
        if (signinRequest == null)
            return Redirect(appConfiguration.ManagementPortalBaseUrl + "/signin");

        // If user not signed in show signin form
        if (User.Identity?.IsAuthenticated != true || signinRequest.UserId == null)
            return View();

        // Check if the user has validated email
        if (!signinRequest.IsEmailVerified)
            return RedirectToAction("VerifyEmail", "EmailVerification");

        // If MFA is enabled redirect to the MFA handler
        if (signinRequest is { IsMfaRequired: true, IsMfaValidated: false })
            return RedirectToAction("MfaInput", "Mfa");

        // If all previos steps of the signin flow are done, redirect to the final authorization endpoint
        // (with skip account chooser flog)
        var oidcRequestParams = signinRequestService.CreateOidcQueryObject(signinRequest);
        oidcRequestParams["chooseAccount"] = "false"; // Set chooseAccount to false, to skip account chooser
        return RedirectToAction("Authorize", "Auth", oidcRequestParams);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> Signin([FromForm] SigninViewModel model, [FromQuery] Guid request,
        CancellationToken cancellationToken)
    {
        var signinRequest = await signinRequestService.GetAsync(request);
        if (signinRequest == null)
            return RedirectToAction("Error", "Error",
                new { error = "Not found", errorDescription = "Signin request not found" });

        // Validate user credentials
        var query = model.Adapt<GetUserByEmailQuery>();
        var user = await bus.InvokeAsync<IdentityUser?>(query, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User with email {email} not found", model.Email);
            ModelState.AddModelError("password", "Incorrect email or password");
            return View();
        }

        // Check if users account is set up
        if (user.IsInvitePending)
        {
            ModelState.AddModelError("password",
                "Your account is not setup yet. Check your email for the invite or contact your administrator.");
            return View();
        }

        // Validate user password
        if (!passwordHasher.Compare(model.Password, user.HashedPassword))
        {
            logger.LogWarning("User with email {email} provided incorrect password", model.Email);
            ModelState.AddModelError("password", "Incorrect email or password");
            return View();
        }

        // Check if the user is not disabled
        if (user.IsDisabled)
        {
            ModelState.AddModelError("password", "Your account has been disabled");
            return View();
        }

        // Create claims for the user
        var claimsIdentity = identityBuilderService.BuildLocalClaimsPrincipal(user, request);
        // Signin in identity server
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties());

        // Update signin request
        signinRequest.UserId = user.Id;
        signinRequest.IsEmailVerified = user.EmailVerified;
        signinRequest.IsMfaRequired = user.IsMFAEnabled;
        signinRequest.IsMfaValidated = false;
        await signinRequestService.UpdateAsync(signinRequest);

        // publish event
        var newEvent = user.Adapt<UserLoggedIn>();
        await bus.PublishAsync(newEvent);
        logger.LogInformation("User {user} logged in.", user.Id);
        
        // Check if the user has verified email
        if (!signinRequest.IsEmailVerified)
            return RedirectToAction("VerifyEmail", "EmailVerification");
        
        // If MFA is enabled and not verified redirect to the MFA handler
        if (signinRequest is { IsMfaRequired: true, IsMfaValidated: false })
            return RedirectToAction("MfaInput", "Mfa");

        // Redirect to authorize endpoint to authorize the client
        var oidcRequestParams = signinRequestService.CreateOidcQueryObject(signinRequest);
        oidcRequestParams["chooseAccount"] = "false"; // Set chooseAccount to false to skip account chooser
        return RedirectToAction("Authorize", "Auth", oidcRequestParams);
    }
}