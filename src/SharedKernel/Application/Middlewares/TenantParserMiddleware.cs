using OpenIddict.Abstractions;
using SharedKernel.Core;
using Wolverine;

namespace SharedKernel.Application.Middlewares;

/// <summary>
/// Middleware to configure the tenant for the rest of the request handling pipeline.
/// by default the tenant id is set from the claims, but if the user is a master admin special tenant query
/// (`tenant={tenantId}`) is allowed.
/// </summary>
public class TenantParserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IMessageBus bus)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            // Try get tenant from query string
            var queryTenant = GetTenantFromQueryString(context);
            if (queryTenant != null && IsMasterAdmin(context))
            {
                bus.TenantId = queryTenant;
            }
            else
            {
                // Try to get tenant from claims
                var tenantId = context.User.Claims
                    .FirstOrDefault(c => c.Type == SharedKernelConstants.TenantClaimName)?.Value;
                if (tenantId != null)
                    bus.TenantId = tenantId;
            }
        }

        // Call the next delegate/middleware in the pipeline.
        await next(context);
    }

    /// <summary>
    /// Helper method to get the tenant from the query string
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private string? GetTenantFromQueryString(HttpContext context)
    {
        var tenantId = context.Request.Query["tenant"].ToString();
        // Check tenant query was provided
        if (string.IsNullOrEmpty(tenantId))
            return null;

        // Check tenant query is a valid guid
        if (!Guid.TryParse(tenantId, out _))
            return null;

        return tenantId;
    }

    /// <summary>
    /// Helper function to get check if the user is a master admin
    /// </summary>
    /// <param name="context"></param>
    /// <returns>true if user is master admin, false otherwise</returns>
    private bool IsMasterAdmin(HttpContext context)
    {
        return context.User.Claims
            .Any(c => c is { Type: OpenIddictConstants.Claims.Role, Value: nameof(UserRole.MasterAdmin) });
    }
}