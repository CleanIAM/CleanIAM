using System.Net;
using System.Security.Claims;
using CleanIAM.SharedKernel.Core;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using OpenIddict.Abstractions;

namespace CleanIAM.SharedKernel.Infrastructure;

/// <summary>
/// Static class containing extension methods for ClaimsPrincipal.
/// </summary>
public static class UserExtensions
{
    /// <summary>
    /// Get and parse the roles of the user from the claims.
    /// </summary>
    /// <param name="user">The Claims principal to get roles from</param>
    /// <returns>List of roles</returns>
    public static UserRole[] GetRoles(this ClaimsPrincipal user)
    {
        var roles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => Enum.Parse<UserRole>(c.Value))
            .ToArray();

        return roles;
    }

    /// <summary>
    /// Get the user ID from the claims principal.
    /// </summary>
    /// <param name="principal">The Claims principal to get user id from</param>
    /// <returns>Id of the user</returns>
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