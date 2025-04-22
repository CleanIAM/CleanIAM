using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SharedKernel.Core.Database;

namespace ManagementPortal;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenIddict(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>()
                    .ReplaceDefaultEntities<Guid>();
            });
        return serviceCollection;
    }
    
    public static IServiceCollection AddOidcAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(
                opts =>
                {
                    opts.Cookie.SameSite = SameSiteMode.None; // Changed from Strict to None for cross-site redirects
                    opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    opts.Cookie.HttpOnly = true;
                }
            )
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                // Use the values from appsettings.json
                options.Authority = "https://localhost:5000";
                options.ClientId = "management-portal";
                options.ClientSecret = "management-portal-secret";
                options.NonceCookie.SameSite = SameSiteMode.None;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ProtocolValidator.RequireNonce = false; //NOTE: Remove inn prod
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.Scope.Add("email");
                options.Scope.Add("roles");
            });
        
        return services;
    }
}