using System.Security.Claims;
using Identity.Api.ViewModels.Shared;
using Identity.Api.ViewModels.Signin;
using Identity.Application.Interfaces;
using Identity.Application.Queries.Users;
using Identity.Core.Events;
using Identity.Core.Users;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Identity.Api.Controllers;

[Route("/")]
public class SigninController(
    ISigninRequestService signinRequestService,
    IIdentityBuilderService identityBuilderService,
    IMessageBus bus,
    IPasswordHasher passwordHasher,
    ILogger logger) : Controller
{
    [HttpGet("signin")]
    public async Task<IActionResult> Signin([FromQuery] Guid request, [FromQuery] string? error)
    {
        ViewData["Error"] = error;
        ViewData["Request"] = request;
        // If user not signed in show signin form
        if (User.Identity?.IsAuthenticated != true)
            return View();

        // TODO: redirect to management portal signin
        var signinRequest = await signinRequestService.GetAsync(request);
        if (signinRequest == null)
            return View("Error", new ErrorViewModel { Error = "Error", ErrorDescription = "Request not found" });

        // Check if the user has validated email
        if (!signinRequest.IsEmailVerified)
            return RedirectToAction("VerifyEmail", "EmailVerification");

        //If MFA is enabled redirect to the MFA handler
        if (signinRequest is { IsMfaRequired: true, IsMfaValidated: false })
            return RedirectToAction("MfaInput", "Mfa");

        return RedirectToAction("Authorize", "Auth", signinRequestService.CreateOidcQueryObject(signinRequest));
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

        // Check if the user has validated email
        if (!user.EmailVerified)
            return RedirectToAction("VerifyEmail", "EmailVerification");


        //If MFA is enabled redirect to the MFA handler
        if (user.IsMFAEnabled)
            // Redirect to MFA handler
            return RedirectToAction("MfaInput", "Mfa");
        
        // publish event
        var newEvent = user.Adapt<UserLoggedIn>();
        await bus.PublishAsync(newEvent);
        logger.LogInformation("User {user} logged in.", user.Id);


        // Redirect to authorize endpoint to authorize the client
        var oidcRequestParams = signinRequestService.CreateOidcQueryObject(signinRequest);
        oidcRequestParams["chooseAccount"] = "false"; // Set chooseAccount to false to skip account chooser
        return RedirectToAction("Authorize", "Auth", oidcRequestParams);
    }
}