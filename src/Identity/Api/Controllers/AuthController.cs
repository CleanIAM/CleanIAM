using System.Security.Claims;
using Identity.Api.ViewModels.Auth;
using Identity.Api.ViewModels.Shared;
using Identity.Application.Interfaces;
using Identity.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Api.Controllers;

[Route("/connect")]
public class AuthController(
    ISigninRequestService signinRequestService,
    IOpenIddictScopeManager scopeManager)
    : Controller
{
    [HttpGet("authorize")]
    public async Task<IActionResult> Authorize()
    {
        //TODO: Refactor identity creation
        var oidcRequest = HttpContext.GetOpenIddictServerRequest();
        if (oidcRequest == null)
            // TODO: Create error page and redirect to it
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidRequest,
                ErrorDescription = "The OpenID Connect request cannot be retrieved."
            });

        // If user not authenticated, redirect to signin
        if (User.Identity?.IsAuthenticated != true)
        {
            var request = new SigninRequest
            {
                Id = Guid.NewGuid(),
                OidcRequest = oidcRequest
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
        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Name, "John Doe"));
        identity.SetScopes(oidcRequest.GetScopes());
        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        // Allow all claims to be added in the access tokens.
        identity.SetDestinations(_ => [OpenIddictConstants.Destinations.AccessToken]);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpGet("endsession")]
    public IActionResult EndSession()
    {
        var parameters = HttpContext.Request.Query.ToArray();
        return View(new EndSessionViewModel
        {
            Parameters = parameters
        });
    }


    [HttpPost("endsession")]
    public async Task<IActionResult> EndSession(OpenIddictRequest request)
    {
        var oidcRequest = HttpContext.GetOpenIddictServerRequest();

        if (oidcRequest == null)
            return View("Error", new ErrorViewModel { Title = "Error", Message = "Invalid end session request." });

        // if (oidcRequest.IdTokenHint == null)
        // {
        //     
        //     return View("Error", new ErrorViewModel { Title = "Error", Message = "Invalid id_token_hint." });
        // }

        // var token = await tokenManager.FindByReferenceIdAsync(oidcRequest.IdTokenHint);


        //
        // // Extract client ID from the id_token_hint
        // var clientId = string.Empty;
        //
        // if (!string.IsNullOrEmpty(request.IdTokenHint))
        // {
        //     // Validate the ID token hint
        //     var validationResult = await _tokenValidator.ValidateTokenAsync(request.IdTokenHint);
        //     if (validationResult.IsValid)
        //         // Extract the client ID from the validated token
        //         clientId = validationResult.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
        // }
        //
        // // If we couldn't get a client ID, reject the request
        // if (string.IsNullOrEmpty(clientId)) return BadRequest("Invalid id_token_hint");
        //
        // // Get the application/client information
        // var application = await _applicationManager.FindByClientIdAsync(clientId);
        //
        // if (application == null) return BadRequest("Invalid client");
        //
        // // Retrieve the registered post-logout redirect URIs for this client
        // var postLogoutRedirectUris = await _applicationManager.GetPostLogoutRedirectUrisAsync(application);
        //
        // // Check if the requested URI is registered
        // var requestedUri = request.PostLogoutRedirectUri;
        //
        // if (string.IsNullOrEmpty(requestedUri) || !postLogoutRedirectUris.Contains(requestedUri))
        //     // Either use a default URI or reject the request
        //     return BadRequest("Invalid post_logout_redirect_uri");

        // Continue with the logout process
        // ...


        await HttpContext.SignOutAsync();

        return Redirect("https://localhost:3000/");
    }
}