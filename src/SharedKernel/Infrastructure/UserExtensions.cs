using System.Security.Claims;
using SharedKernel.Core;

namespace SharedKernel.Infrastructure;

public static class UserExtensions
{
    public static UserRole[] GetRoles(this ClaimsPrincipal user)
    {
        var roles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => Enum.Parse<UserRole>(c.Value))
            .ToArray();

        return roles;
    }
}