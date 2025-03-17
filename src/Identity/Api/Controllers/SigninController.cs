using System.Security.Claims;
using Identity.Api.ViewModels.Shared;
using Identity.Api.ViewModels.Signin;
using Identity.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("/")]
public class SigninController(ISigninRequestService signinRequestService): Controller
{
    [HttpGet("signin")]
    public async Task<IActionResult> Signin([FromQuery] Guid request)
    {
        
        var signinRequest = await signinRequestService.GetAsync(request);
        
        // If signin without oidc request, show error
        // TODO: If no oidc request, redirect to console signin.
        if(signinRequest == null)
        {
            return View("Error", new ErrorViewModel{ Title = "Error", Message = "Request not found" });
        }

        // If user already authenticated redirect to authorize
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Authorize", "Auth", signinRequestService.CreateOIDCQueryObject(signinRequest) );
        }


        return View();
    }

    [HttpPost("signin")]
    public async Task<IActionResult> Signin([FromForm] SigninViewModel model, [FromQuery] Guid request)
    {
        var signinRequest = await signinRequestService.GetAsync(request);
        
        // If signin without oidc request, show error
        if(signinRequest == null)
        {
            return View("Error", new ErrorViewModel{ Title = "Error", Message = "Request not found" });
        }
        
        // TODO: Validate user credentials
        
        
        // Create claims for the user
        
        
        var user = new
        {
            Email = "admin@localhost",
            FullName = "Administrator"
        };

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new("FullName", user.FullName),
            new(ClaimTypes.Role, "Administrator")
        };

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

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        
        return RedirectToAction("Authorize", "Auth", signinRequestService.CreateOIDCQueryObject(signinRequest));
    }


    
    [HttpGet("test")]
    public async Task<IResult> Test()
    {
        
        if (User.Identity?.IsAuthenticated != true)
        {
            return Results.Ok("Not Authenticated");
        
        }
        return Results.Ok("Authenticated: " + User.Identity.Name);
    }
    
    
}