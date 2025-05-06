using System.Collections.Immutable;
using System.Net;
using System.Security.Claims;
using Identity.Application.Interfaces;
using Identity.Application.Queries.Users;
using Identity.Core.Users;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using SharedKernel;
using SharedKernel.Infrastructure;
using Wolverine;

namespace Identity.Infrastructure.Services;

public class IdentityBuilderService(IOpenIddictScopeManager scopeManager, IMessageBus bus) : IIdentityBuilderService
{
    public async Task<Result<ClaimsPrincipal>> BuildClaimsPrincipalAsync(OpenIddictRequest request, Guid userId)
    {
        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role);

        var scopes = request.GetScopes();

        var claims = await BuildClaimsAsync(userId, scopes);
        if (claims.IsError())
            return Result.From(claims);

        // Add the claims that will be persisted in the tokens.
        identity.AddClaims(claims.Value);
        identity.SetScopes(request.GetScopes());
        identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

        // Allow all claims to be added in the access tokens.
        identity.SetDestinations(_ => [OpenIddictConstants.Destinations.AccessToken]);

        return Result.Ok(new ClaimsPrincipal(identity));
    }

    public async Task<Result<IEnumerable<Claim>>> BuildClaimsAsync(Guid userId, ImmutableArray<string> scopes)
    {
        // Get user from database
        var query = new GetUserByIdQuery(userId);
        var user = await bus.InvokeAsync<IdentityUser?>(query);
        if (user == null)
            return Result.Error("User not found", HttpStatusCode.BadRequest);

        // Build user info claims
        var claims = new List<Claim>
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            new(OpenIddictConstants.Claims.Subject, user.Id.ToString()),
            // Add the "tenant" claim to the identity, to enable multi-tenancy.
            new(SharedKernelConstants.TenantClaimName, user.TenantId.ToString())
        };

        if (scopes.Contains(OpenIddictConstants.Scopes.Profile))
        {
            claims.Add(new Claim(OpenIddictConstants.Claims.Name, $"{user.FirstName} {user.LastName}"));
            claims.Add(new Claim(OpenIddictConstants.Claims.GivenName, user.FirstName));
            claims.Add(new Claim(OpenIddictConstants.Claims.FamilyName, user.LastName));
        }

        if (scopes.Contains(OpenIddictConstants.Scopes.Email))
        {
            claims.Add(new Claim(OpenIddictConstants.Claims.Email, user.Email));
            claims.Add(new Claim(OpenIddictConstants.Claims.EmailVerified, user.EmailVerified.ToString()));
        }

        if (scopes.Contains(OpenIddictConstants.Scopes.Roles))
            foreach (var role in user.Roles)
                claims.Add(new Claim(OpenIddictConstants.Claims.Role, role.ToString()));

        return Result.Ok((IEnumerable<Claim>)claims);
    }
}