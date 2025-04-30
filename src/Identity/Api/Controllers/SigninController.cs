using System.Security.Claims;
using Identity.Api.ViewModels.Shared;
using Identity.Api.ViewModels.Signin;
using Identity.Application.Interfaces;
using Identity.Application.Queries.Users;
using Identity.Core.Users;
using Identity.Infrastructure;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using Wolverine;

namespace Identity.Api.Controllers;

[Route("/")]
public class SigninController(
    ISigninRequestService signinRequestService,
    IMessageBus bus,
    IPasswordHasher passwordHasher) : Controller
{
    [HttpGet("signin")]
    public async Task<IActionResult> Signin([FromQuery] Guid request, [FromQuery] string? error)
    {
        ViewData["Error"] = error;
        ViewData["Request"] = request;
        // If user not signed in show signin form
        if (User.Identity?.IsAuthenticated != true)
            return View();

        var signinRequest = await signinRequestService.GetAsync(request);

        // TODO: If no oidc request, redirect to console signin.
        // If signin without oidc request, show error
        if (signinRequest == null)
            return View("Error", new ErrorViewModel { Error = "Error", ErrorDescription = "Request not found" });

        // If user already authenticated redirect to authorize
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
        var user = await bus.InvokeAsync<User?>(query, cancellationToken);

        if (user == null)
        {
            ModelState.AddModelError("password", "Incorrect email or password");
            return View();
        }

        // Validate user password
        if (!passwordHasher.Compare(model.Password, user.HashedPassword))
        {
            ModelState.AddModelError("password", "Incorrect email or password");
            return View();
        }


        // Create claims for the user
        var claims = new Claim[]
        {
            new(OpenIddictConstants.Claims.Subject, user.Id.ToString()),
            new(IdentityConstants.SigninRequestClaimName, request.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties();

        // Signin in identity server
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Update signin request
        signinRequest.UserId = user.Id;
        await signinRequestService.UpdateAsync(signinRequest);
        

        // Check if the user has validated email
        if (!user.EmailVerified)
            return RedirectToAction("VerifyEmail", "EmailVerification");

        
        //TODO: If MFA is enabled redirect to the MFA handler


        // Redirect to authorize endpoint to authorize the client
        return RedirectToAction("Authorize", "Auth", signinRequestService.CreateOidcQueryObject(signinRequest));
    }

    [HttpGet("test")]
    public async Task<IResult> Test()
    {
        if (User.Identity?.IsAuthenticated != true)
            return Results.Ok("Not Authenticated");
        return Results.Ok("Authenticated: " + User.Identity.Name);
    }
}