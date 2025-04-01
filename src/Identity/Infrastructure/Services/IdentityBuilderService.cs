using System.Security.Claims;
using Identity.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;

namespace Identity.Infrastructure.Services;

public class IdentityBuilderService(IOpenIddictScopeManager scopeManager) : IIdentityBuilderService
{
    public async Task<ClaimsPrincipal> BuildClaimsPrincipalAsync(OpenIddictRequest request, Guid userId)
    {
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role);

        // Add the claims that will be persisted in the tokens.
        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, Guid.NewGuid().ToString()));
        identity.AddClaim(new Claim(OpenIddictConstants.Claims.Name, "John Doe"));
        identity.SetScopes(request.GetScopes());
        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        // Allow all claims to be added in the access tokens.
        identity.SetDestinations(_ => [OpenIddictConstants.Destinations.AccessToken]);

        return new ClaimsPrincipal(identity);
    }
}