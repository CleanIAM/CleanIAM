using System.Security.Claims;
using Identity.Core;
using Identity.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Api.Controllers;

[Route("/connect")]
public class AuthController(ISigninRequestService signinRequestService) : Controller
{

    [HttpGet("authorize")]
    public async Task<IActionResult> Authorize()
    {
        
        //TODO: Refactor identity creation
        var oidcRequest = HttpContext.GetOpenIddictServerRequest();
        if (oidcRequest == null)
        {
            // TODO: Create error page and redirect to it
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidRequest,
                ErrorDescription = "The OpenID Connect request cannot be retrieved."
            });
        }

        // If user not authenticated, redirect to signin
        if (User.Identity?.IsAuthenticated != true)
        {
            var request = new SigninRequest
            {
                Id = Guid.NewGuid(),
                oidcRequest = oidcRequest
            };
            await signinRequestService.SaveAsync(request);
            return RedirectToAction("Signin", "Signin", new { request = request.Id });
        }
        
        //TODO: show prompt to authorize the client
        
        // Signin the resource owner to the client
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role);

        // Add the claims that will be persisted in the tokens.
        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, Guid.NewGuid().ToString()));

        identity.SetScopes(oidcRequest.GetScopes());
        // Allow all claims to be added in the access tokens.
        identity.SetDestinations(_ => [OpenIddictConstants.Destinations.AccessToken]);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    
    [HttpGet("endsession")]
    public async Task<IActionResult> Signout([FromQuery] string clientId, [FromQuery] string post_logout_redirect_uri)
    {
        //TODO: Check if the given client allows given redirect uri
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        return Redirect(post_logout_redirect_uri);
    }
}