using System.Net;
using System.Security.Claims;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure;

namespace ManagementPortal.Infrastructure.Utils;

public static class UserExtensions
{
    public static Result<Guid> GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(OpenIddictConstants.Claims.Subject);
        if (userIdClaim == null)
            return Result.Error("User ID claim not found.", HttpStatusCode.Unauthorized);

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
            return Result.Error("User ID claim is not a valid GUID.", HttpStatusCode.Unauthorized);

        return Result.Ok(userId);
    }
}