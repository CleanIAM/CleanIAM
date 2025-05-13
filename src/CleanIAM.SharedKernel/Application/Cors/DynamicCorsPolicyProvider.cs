using CleanIAM.SharedKernel.Application.Interfaces;
using FastExpressionCompiler;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace CleanIAM.SharedKernel.Application.Cors;

/// <summary>
/// This class is used to provide a CORS policy for OpenId Connect requests.
/// </summary>
public class DynamicCorsPolicyProvider(IAppConfiguration appConfiguration) : ICorsPolicyProvider
{
    /// <summary>
    /// This method is called to get a CORS policy for a given request.
    /// </summary>
    public Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? _)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();

        // Set custom CORS for some OpenId Connect requests
        if (path != null && (
                path.Contains("/.well-known/openid-configuration") || 
                path.Contains("/connect/token") ||
                path.Contains("/connect/introspect") ||
                path.Contains("/connect/userinfo")))
        {
            var origin = context.Request.Headers.Origin.GetFirst();
            
            return Task.FromResult<CorsPolicy?>(new CorsPolicyBuilder()
                .WithOrigins(origin ?? "")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .Build());
        }

        return Task.FromResult<CorsPolicy?>(new CorsPolicyBuilder()
            .WithOrigins(appConfiguration.ManagementPortalBaseUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .Build());
    }
}
