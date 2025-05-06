using Identity.Api.ViewModels.Auth;
using Identity.Api.ViewModels.Shared;
using Identity.Application.Interfaces;
using Identity.Core.Requests;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Api.Controllers;

/// <summary>
/// Main Controller for handling OAuth2 and OpenId Connect requests.
/// Some parts of the OAuth 2 flow are handled by OpenIddict itself. <see cref="DependencyInjection.AddOpenIddict" />
/// </summary>
[Route("/connect")]
public class AuthController(
    ISigninRequestService signinRequestService,
    IIdentityBuilderService identityBuilderService)
    : Controller
{
    /// <summary>
    /// The main endpoint for OAuth 2 authorization code flow.
    /// If the user is not authenticated, the user will be redirected to the signin page.
    /// </summary>
    [HttpGet("authorize")]
    public async Task<IActionResult> Authorize()
    {
        var oidcRequest = HttpContext.GetOpenIddictServerRequest();
        if (oidcRequest == null)
            // TODO: Create error page and redirect to it
            return View("error", new ErrorViewModel
            {
                Error = OpenIddictConstants.Errors.InvalidRequest,
                ErrorDescription = "The OpenID Connect request cannot be retrieved."
            });

        // If user not authenticated, redirect to signin
        var userIdRaw = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
        if (User.Identity?.IsAuthenticated != true || !Guid.TryParse(userIdRaw, out var userId))
        {
            var request = new SigninRequest
            {
                Id = Guid.NewGuid(),
                OidcRequest = oidcRequest
            };
            await signinRequestService.SaveAsync(request);
            return RedirectToAction("Signin", "Signin", new { request = request.Id });
        }


        var principal = await identityBuilderService.BuildClaimsPrincipalAsync(oidcRequest, userId);
        if (principal.IsError())
        {
            await HttpContext.SignOutAsync();
            var request = new SigninRequest
            {
                Id = Guid.NewGuid(),
                OidcRequest = oidcRequest
            };
            await signinRequestService.SaveAsync(request);
            return RedirectToAction("Signin", "Signin", new { request = request.Id });
        }

        return SignIn(principal.Value, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Show the view to confirm the consent of the user to sing out.
    /// </summary>
    /// <returns></returns>
    [HttpGet("endsession")]
    public IActionResult EndSession()
    {
        var parameters = HttpContext.Request.Query.ToArray();
        return View(new EndSessionViewModel
        {
            Parameters = parameters
        });
    }

    /// <summary>
    /// Endpoint handling the sign-out request.
    /// </summary>
    [HttpPost("endsession")]
    public async Task<IActionResult> EndSession(OpenIddictRequest request, EndSessionViewModel? model)
    {
        var oidcRequest = HttpContext.GetOpenIddictServerRequest();

        // Sign out from a local identity server session
        if (model is not null && model.FullLogout)
            await HttpContext.SignOutAsync();

        // Sign out from the OpenIddict session
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                // The `oidcRequest?.PostLogoutRedirectUri` is already validated by OpenIddict.
                RedirectUri = oidcRequest?.PostLogoutRedirectUri ?? "/connect/endsession/success"
            });
    }

    /// <summary>
    /// Show the view to confirm the consent of the user to sing out.
    /// Just a fallback in case the client application did not provide a redirect URI.
    /// </summary>
    [HttpGet("endsession/success")]
    public IActionResult EndSessionSuccess()
    {
        return View();
    }

    /// <summary>
    /// The main endpoint for OpenId Connect userinfo requests.
    /// </summary>
    [HttpGet("userinfo")]
    [HttpPost("userinfo")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Userinfo()
    {
        // Get user id from claims
        var userIdRaw = User.GetClaim(OpenIddictConstants.Claims.Subject);
        if (!Guid.TryParse(userIdRaw, out var userId))
            return BadRequest("Invalid sub claim, expected UUID format.");

        var scopes = User.GetScopes();

        var claims = await identityBuilderService.BuildClaimsAsync(userId, scopes);

        if (claims.IsError())
            return BadRequest(claims.ErrorValue.Message);

        var parsableClaims = claims.Value.GroupBy(c => c.Type)
            .ToDictionary(
                g => g.Key,
                g => g.Select(c => c.Value).ToList()
            );

        // The object must be normalize to make it usable json
        // If the claim has only one value, we need to convert it to a single value
        var normalized = parsableClaims.ToDictionary(
            kvp => kvp.Key,
            kvp => (object)(kvp.Value.Count == 1 ? kvp.Value[0] : kvp.Value)
        );

        return Ok(normalized);
    }
}