using System.Diagnostics.CodeAnalysis;
using Identity.Application.Interfaces;
using Identity.Core.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Abstractions;

namespace Identity.Api.Controllers;

using System.Security.Claims;
using Marten;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.WebIntegration;


/// <summary>
/// Controller with endpoints handling external providers authentication such as Microsoft or Google
/// </summary>
[Route("/external-providers")]
public class ExternalSigninProvidersController(
    IQuerySession session, ISigninRequestService signinRequestService) : Controller
{
    /// <summary>
    /// Initiates the authentication process with Microsoft provider
    /// </summary>
    [HttpGet("request/{provider}")]
    public async Task<IResult> ExternalSignin([FromQuery] Guid request, [FromRoute] string provider)
    {
        if (request == Guid.Empty)
            RedirectToSignin("Request id is invalid");
        var signinRequest = await signinRequestService.GetAsync(request);
        if (signinRequest == null)
            RedirectToSignin("Request not found");

        TempData["RequestId"] = request;
        
        // Return challenge to the external provider
        return Results.Challenge(null, [
            provider switch
        {
            OpenIddictClientWebIntegrationConstants.Providers.Microsoft => OpenIddictClientWebIntegrationConstants.Providers.Microsoft,
            OpenIddictClientWebIntegrationConstants.Providers.Google => OpenIddictClientWebIntegrationConstants.Providers.Google,
            _ => throw new NotSupportedException("Provider not supported")
        }]
            );
    }

    /// <summary>
    /// Finalizes the authentication process with external singin provider
    /// </summary>
    [HttpGet("callback/{provider}")]
    public async Task<IActionResult> ExternalSigninCallback([FromRoute]string provider)
    {
        var requestId = (Guid?)TempData["RequestId"];
        
        var context = HttpContext;
        var result = await context.AuthenticateAsync(OpenIddictClientWebIntegrationConstants.Providers.Microsoft);
        if (!result.Succeeded)
            return RedirectToSignin("External authentication failed", requestId);
        

        // Validate the authentication result
        var receivedClaims = result.Principal.Claims;
        var email = receivedClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = await session.Query<User>().FirstOrDefaultAsync(u => u.AnyTenant() && u.Email == email);

        
        if (user == null)
            return RedirectToSignin("User doesn't exist", requestId);
        
        // Sign in user
        // Create claims for the user
        var claims = new Claim[] { new(OpenIddictConstants.Claims.Subject, user.Id.ToString()) };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties();

        // Signin in identity server
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        var signinRequest = await signinRequestService.GetAsync(requestId.Value);
        if(signinRequest == null)
            return RedirectToSignin("Request id is invalid");
        
        // Redirect to authorize endpoint to authorize the client
        return RedirectToAction("Authorize", "Auth", signinRequestService.CreateOidcQueryObject(signinRequest));

        
    }


    private IActionResult RedirectToSignin(string? error = null, Guid? request = null)
    {
        if (error != null)
            return RedirectToAction("Signin", "Signin", new { Error = error, Request = request });
        return RedirectToAction("Signin", "Signin");
    }
}