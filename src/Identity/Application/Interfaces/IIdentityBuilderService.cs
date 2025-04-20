using System.Collections.Immutable;
using System.Security.Claims;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure;

namespace Identity.Application.Interfaces;

public interface IIdentityBuilderService
{
    Task<Result<ClaimsPrincipal>> BuildClaimsPrincipalAsync(OpenIddictRequest request, Guid userId);

    Task<Result<IEnumerable<Claim>>> BuildClaimsAsync(Guid userId, ImmutableArray<string> scopes);

}