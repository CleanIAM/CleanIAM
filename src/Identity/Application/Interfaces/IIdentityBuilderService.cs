using System.Collections.Immutable;
using System.Security.Claims;
using Identity.Core.Users;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure;

namespace Identity.Application.Interfaces;

public interface IIdentityBuilderService
{
    /// <summary>
    /// Builds a claims principal from the OpenIddict request and user id.
    /// </summary>
    /// <param name="request">The Oidc request values</param>
    /// <param name="userId">Id of the user</param>
    /// <returns></returns>
    Task<Result<ClaimsPrincipal>> BuildClaimsPrincipalAsync(OpenIddictRequest request, Guid userId);

    /// <summary>
    /// Builds the claims for the user based on the scopes requested.
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <param name="scopes">Scopes</param>
    /// <returns></returns>
    Task<Result<IEnumerable<Claim>>> BuildClaimsAsync(Guid userId, ImmutableArray<string> scopes);

    /// <summary>
    /// build claims principal for local IAM authentication
    /// </summary>
    /// <param name="user">User the claims should be built for</param>
    /// <param name="requestId">Id of the signin request</param>
    /// <returns></returns>
    public ClaimsIdentity BuildLocalClaimsPrincipal(IdentityUser user, Guid requestId);
}