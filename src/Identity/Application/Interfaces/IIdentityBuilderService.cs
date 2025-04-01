using System.Security.Claims;
using OpenIddict.Abstractions;

namespace Identity.Application.Interfaces;

public interface IIdentityBuilderService
{
    Task<ClaimsPrincipal> BuildClaimsPrincipalAsync(OpenIddictRequest request, Guid userId);
}