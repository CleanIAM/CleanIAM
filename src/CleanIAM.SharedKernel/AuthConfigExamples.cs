using System.IdentityModel.Tokens.Jwt;
using CleanIAM.SharedKernel.Core.Database;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace CleanIAM.SharedKernel;

public static class AuthConfigExamples
{
    /// <summary>
    /// Configure OpenIddict validation with remote Authorization server.
    /// </summary>
    /// <remarks>
    /// <b> Only use when CleanIAM.Identity is not included in application</b> <br />
    /// </remarks>
    public static IServiceCollection AddOpenIddictValidations(this IServiceCollection services,
        IConfiguration configuration)
    {
        var authority = configuration.GetSection("Authentication")["Authority"];
        var clientId = configuration.GetSection("Authentication")["ClientId"];
        var clientSecret = configuration.GetSection("Authentication")["ClientSecret"];
        var resourceServerId = configuration.GetSection("Authentication")["ResourceServerId"];

        Guard.IsNotNullOrEmpty(authority, "The authority server url is not set");
        Guard.IsNotNullOrEmpty(clientId, "The auth client id is not set");
        Guard.IsNotNullOrEmpty(clientSecret, "The auth client secret is not set");
        Guard.IsNotNullOrEmpty(resourceServerId, "The resource server id is not set");


        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>()
                    .ReplaceDefaultEntities<Guid>();
            })
            .AddValidation(options =>
            {
                // Note: the validation handler uses OpenID Connect discovery
                // to retrieve the address of the introspection endpoint.
                options.SetIssuer(authority);
                options.AddAudiences(resourceServerId);

                // Configure the validation handler to use introspection and register the client
                // credentials used when communicating with the remote introspection endpoint.
                options.UseIntrospection()
                    .SetClientId(clientId)
                    .SetClientSecret(clientSecret);

                // Register the System.Net.Http integration.
                options.UseSystemNetHttp();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();

                // Enable authorization entry validation, which is required to be able
                // to reject access tokens retrieved from a revoked authorization code.
                options.EnableAuthorizationEntryValidation();
            });
        return services;
    }

    /// <summary>
    /// This method configures the openiddict authentication scheme for the management portal.
    /// This is the example usage of OpenIdConnect authentication:
    /// </summary>
    public static IServiceCollection AddOidcAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(opts =>
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