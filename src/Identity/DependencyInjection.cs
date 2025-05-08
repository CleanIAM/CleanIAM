using System.Text;
using CommunityToolkit.Diagnostics;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Core.Database;

namespace Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityProject(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddScoped<ISigninRequestService, SigninRequestService>();
        serviceCollection.AddTransient<IPasswordHasher, PasswordHasher>();
        serviceCollection.AddTransient<IIdentityBuilderService, IdentityBuilderService>();
        serviceCollection.AddScoped<IEmailService, CoravelEmailService>();


        return serviceCollection;
    }

    public static IServiceCollection AddOpenIddict(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var encryptionKey = configuration.GetSection("OpenIddict")["EncryptionKey"];
        var identityBaseUrl = configuration.GetSection("HttpRoutes")["IdentityBaseUrl"];

        // External signin provides
        var microsoftClientId =
            configuration.GetSection("Authentication:OpenIddict:ExternalProviders:Microsoft")["ClientId"];
        var microsoftClientSecret =
            configuration.GetSection("Authentication:OpenIddict:ExternalProviders:Microsoft")["ClientSecret"];
        var googleClientId =
            configuration.GetSection("Authentication:OpenIddict:ExternalProviders:Google")["ClientId"];
        var googleClientSecret =
            configuration.GetSection("Authentication:OpenIddict:ExternalProviders:Google")["ClientSecret"];


        Guard.IsNotNullOrEmpty(encryptionKey, "Encryption key");
        Guard.IsNotNullOrEmpty(identityBaseUrl, "Identity Base Url");
        Guard.IsNotNullOrEmpty(microsoftClientId, "Microsoft Client Id");
        Guard.IsNotNullOrEmpty(microsoftClientSecret, "Microsoft Client Secret");
        Guard.IsNotNullOrEmpty(googleClientId, "Google Client Id");
        Guard.IsNotNullOrEmpty(googleClientSecret, "Google Client Secret");


        serviceCollection.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>()
                    .ReplaceDefaultEntities<Guid>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize")
                    .SetTokenEndpointUris("/connect/token")
                    .SetIntrospectionEndpointUris("connect/introspect")
                    .SetEndSessionEndpointUris("/connect/endsession")
                    .SetUserInfoEndpointUris("/connect/userinfo");

                // .SetDeviceAuthorizationEndpointUris("connect/device")
                // .SetEndUserVerificationEndpointUris("connect/verify")
                // .SetPushedAuthorizationEndpointUris("connect/par")
                // .SetRevocationEndpointUris("connect/revoke")
                // .SetUserInfoEndpointUris("connect/userinfo");

                options.AllowAuthorizationCodeFlow(); // For FE clients
                options.AllowClientCredentialsFlow() // For BE clients
                    .RequireProofKeyForCodeExchange();
                options.AllowRefreshTokenFlow();

                options.AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableEndSessionEndpointPassthrough()
                    .EnableUserInfoEndpointPassthrough()
                    .EnableStatusCodePagesIntegration();

                options.DisableAccessTokenEncryption();

                options.AddEncryptionKey(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(encryptionKey)));
            })
            .AddClient(options =>
            {
                // Allow the OpenIddict client to negotiate the authorization code flow.
                options.AllowAuthorizationCodeFlow();

                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                    .EnableRedirectionEndpointPassthrough();

                options.UseWebProviders()
                    .AddMicrosoft(config =>
                    {
                        config.SetClientId(microsoftClientId);
                        config.SetClientSecret(microsoftClientSecret);
                        config.SetRedirectUri("external-providers/callback/microsoft");
                        config.AddScopes("email", "profile", "openid");
                    })
                    .AddGoogle(config =>
                    {
                        config.SetClientId(googleClientId);
                        config.SetClientSecret(googleClientSecret);
                        config.SetRedirectUri("external-providers/callback/google");
                        config.AddScopes("email", "profile", "openid");
                    });
            })
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();

                // Enable authorization entry validation, which is required to be able
                // to reject access tokens retrieved from a revoked authorization code.
                options.EnableAuthorizationEntryValidation();
            });
        ;
        return serviceCollection;
    }
}