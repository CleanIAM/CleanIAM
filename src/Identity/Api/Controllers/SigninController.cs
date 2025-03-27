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
    public async Task<IActionResult> Signin([FromQuery] Guid request)
    {
        // If user not signed in show signin form
        if (User.Identity?.IsAuthenticated != true)
            return View();

        var signinRequest = await signinRequestService.GetAsync(request);

        // TODO: If no oidc request, redirect to console signin.
        // If signin without oidc request, show error
        if (signinRequest == null)
            return View("Error", new ErrorViewModel { Title = "Error", Message = "Request not found" });

        // If user already authenticated redirect to authorize
        return RedirectToAction("Authorize", "Auth", signinRequestService.CreateOidcQueryObject(signinRequest));
    }

    [HttpPost("signin")]
    public async Task<IActionResult> Signin([FromForm] SigninViewModel model, [FromQuery] Guid request,
        CancellationToken cancellationToken)
    {
        var signinRequest = await signinRequestService.GetAsync(request);

        // If signin without oidc request, show error

        // TODO: Validate user credentials
        var query = model.Adapt<GetUserByEmailQuery>();
        var user = await bus.InvokeAsync<User?>(query, cancellationToken);

        if (user == null)
        {
            ModelState.AddModelError("password", "Incorrect email or password");
            return View();
        }

        if (!passwordHasher.Compare(model.Password, user.HashedPassword))
        {
            ModelState.AddModelError("password", "Incorrect email or password");
            return View();
        }


        // Create claims for the user
        var claims = new Claim[] { new(OpenIddictConstants.Claims.Subject, user.Id.ToString()) };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            //AllowRefresh = <bool>,
            // Refreshing the authentication session should be allowed.

            //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            // The time at which the authentication ticket expires. A 
            // value set here overrides the ExpireTimeSpan option of 
            // CookieAuthenticationOptions set with AddCookie.

            //IsPersistent = true,
            // Whether the authentication session is persisted across 
            // multiple requests. When used with cookies, controls
            // whether the cookie's lifetime is absolute (matching the
            // lifetime of the authentication ticket) or session-based.

            //IssuedUtc = <DateTimeOffset>,
            // The time at which the authentication ticket was issued.

            //RedirectUri = <string>
            // The full path or absolute URI to be used as an http 
            // redirect response value.
        };

        // Signin in identity server
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        //TODO: If MFA is enabled redirect to the MFA handler


        if (signinRequest == null)
            return RedirectToAction("Home", "Console");

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