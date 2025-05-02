using Identity.Api.ViewModels.Auth;
using Identity.Application.Interfaces;
using Identity.Core.Requests;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Api.Controllers;

[Route("/connect")]
public class AuthController(
    ISigninRequestService signinRequestService,
    IIdentityBuilderService identityBuilderService)
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
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidRequest,
                ErrorDescription = principal.ErrorValue.Message
            });

        return SignIn(principal.Value, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
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

        // Sign out from local identity server session
        await HttpContext.SignOutAsync();

        // Sign out from OpenIddict session
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                RedirectUri = oidcRequest?.RedirectUri ?? "/",
            });
    }

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

        var parsableClaims = claims.Value.Select(claim => (claim.Type, claim.Value)).ToDictionary();

        return Ok(parsableClaims);
    }
}