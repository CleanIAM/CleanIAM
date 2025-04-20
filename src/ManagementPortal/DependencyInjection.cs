using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SharedKernel.Core.Database;
using Microsoft.IdentityModel.Tokens;

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
                    opts.Cookie.SameSite = SameSiteMode.Strict;
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
                
                //
                // options.Authority = oidcConfig["Authority"];
                // options.ClientId = oidcConfig["ClientId"];
                // options.ClientSecret = oidcConfig["ClientSecret"];

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                
                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                options.TokenValidationParameters.RoleClaimType = "roles";
                
                options.ResponseType = OpenIdConnectResponseType.Code;
                // options.ResponseMode = OpenIdConnectResponseMode.FormPost;
                // options.SaveTokens = true;
                // options.UseTokenLifetime = true;
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("roles");
                //
                // // Map claims
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
                //
                // // Handle events
                // options.Events = new OpenIdConnectEvents
                // {
                //     OnAccessDenied = context =>
                //     {
                //         context.Response.Redirect("/");
                //         context.HandleResponse();
                //         return Task.CompletedTask;
                //     }
                // };
            });
        
        return services;
    }
}