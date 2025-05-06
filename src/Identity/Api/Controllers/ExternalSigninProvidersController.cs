using System.Security.Claims;
using Identity.Application.Interfaces;
using Identity.Core.Users;
using Marten;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.WebIntegration;

namespace Identity.Api.Controllers;

/// <summary>
/// Controller with endpoints handling external providers authentication such as Microsoft or Google
/// </summary>
[Route("/external-providers")]
public class ExternalSigninProvidersController(
    IQuerySession session,
    ISigninRequestService signinRequestService,
    IIdentityBuilderService identityBuilderService) : Controller
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
                    OpenIddictClientWebIntegrationConstants.Providers.Microsoft =>
                        OpenIddictClientWebIntegrationConstants.Providers.Microsoft,
                    OpenIddictClientWebIntegrationConstants.Providers.Google => OpenIddictClientWebIntegrationConstants
                        .Providers.Google,
                    _ => throw new NotSupportedException("Provider not supported")
                }
            ]
        );
    }

    /// <summary>
    /// Finalizes the authentication process with external signin provider
    /// </summary>
    [HttpGet("callback/{provider}")]
    public async Task<IActionResult> ExternalSigninCallback([FromRoute] string provider)
    {
        var requestId = (Guid?)TempData["RequestId"];

        var context = HttpContext;
        var result = await context.AuthenticateAsync(OpenIddictClientWebIntegrationConstants.Providers.Microsoft);
        if (!result.Succeeded)
            return RedirectToSignin("External authentication failed", requestId);


        // Validate the authentication result
        var receivedClaims = result.Principal.Claims;
        var email = receivedClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = await session.Query<IdentityUser>().FirstOrDefaultAsync(u => u.AnyTenant() && u.Email == email);


        if (user == null)
            return RedirectToSignin("User doesn't exist", requestId);

        // Sign in user
        // Create claims for the user
        var claimsIdentity = identityBuilderService.BuildLocalClaimsPrincipal(user, requestId ?? Guid.Empty);

        // Signin in identity server
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties());

        var signinRequest = await signinRequestService.GetAsync(requestId.Value);
        if (signinRequest == null)
            return RedirectToSignin("Request id is invalid");

        // Redirect to authorize endpoint to authorize the client
        var oidcRequestParams = signinRequestService.CreateOidcQueryObject(signinRequest);
        oidcRequestParams["chooseAccount"] = "false"; // Set chooseAccount to false to skip account chooser
        return RedirectToAction("Authorize", "Auth", oidcRequestParams);
    }


    /// <summary>
    /// Helper function to redirect to the signin page with the error message
    /// </summary>
    /// <param name="error">The error message</param>
    /// <param name="request">Id of the signin request</param>
    /// <returns></returns>
    private IActionResult RedirectToSignin(string? error = null, Guid? request = null)
    {
        if (error != null)
            return RedirectToAction("Signin", "Signin", new { Error = error, Request = request });
        return RedirectToAction("Signin", "Signin");
    }
}