using System.Text;
using CommunityToolkit.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using SharedKernel.Core.Database;

namespace Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenIddict(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var encryptionKey = configuration.GetSection("OpenIddict")["EncryptionKey"];
        var identityBaseUrl = configuration.GetSection("HttpRoutes")["IdentityBaseUrl"];

        // External signin provides
        var microsoftClientId =
            configuration.GetSection("Authentication:OpenIddict:ExternalProviders:Microsoft")["ClientId"];
        microsoftClientId = Environment.GetEnvironmentVariable("OpenIddict__ExternalProviders__Microsoft__ClientId");
        var microsoftClientSecret =
            configuration.GetSection("Authentication:OpenIddict:ExternalProviders:Microsoft")["ClientSecret"];
        microsoftClientSecret = Environment.GetEnvironmentVariable("OpenIddict__ExternalProviders__Microsoft__ClientSecret");

        var googleClientId = configuration.GetSection("Authentication:OpenIddict:ExternalProviders:Google")["ClientId"];
        googleClientId = Environment.GetEnvironmentVariable("OpenIddict__ExternalProviders__Google__ClientId");
        var googleClientSecret =
            configuration.GetSection("Authentication:OpenIddict:ExternalProviders:Google")["ClientSecret"];
        googleClientSecret = Environment.GetEnvironmentVariable("OpenIddict__ExternalProviders__Google__ClientSecret");
        
        
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
                    .SetEndSessionEndpointUris("/connect/endsession").
                    SetUserInfoEndpointUris("/connect/userinfo");

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
                        config.SetRedirectUri($"external-providers/callback/google");
                        config.AddScopes("email", "profile", "openid");
                    });
            });
        return serviceCollection;
    }

    public static async Task ConfigureOpenIddict(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();

        await CreateExampleOidcClient(scope);
        await CreateDefaultOidcScopes(scope);
    }

    private static async Task CreateExampleOidcClient(AsyncServiceScope scope)
    {
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var feClient = new OpenIddictApplicationDescriptor
        {
            ClientId = "example-FE-client",
            ClientType = OpenIddictConstants.ClientTypes.Public,
            RedirectUris =
            {
                new Uri("https://localhost:3000/signin-callback")
            },
            PostLogoutRedirectUris = { new Uri("https://localhost:3000/signout-callback") },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + "BE1"
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        };

        if (await applicationManager.FindByClientIdAsync("example-FE-client") is null)
            await applicationManager.CreateAsync(feClient);
        
        var managementConsoleFrontend = new OpenIddictApplicationDescriptor
        {
            ClientId = "management-console-fe-client",
            ClientType = OpenIddictConstants.ClientTypes.Public,
            RedirectUris =
            {
                new Uri("https://localhost:3000/signin-callback")
            },
            PostLogoutRedirectUris = { new Uri("https://localhost:3000/signout-callback") },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + "BE1"
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        };

        if (await applicationManager.FindByClientIdAsync("management-console-fe-client") is null)
            await applicationManager.CreateAsync(managementConsoleFrontend);


        var beClient = new OpenIddictApplicationDescriptor
        {
            ClientId = "example-BE-client",
            ClientSecret = "test-secret",
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Introspection
            }
        };

        if (await applicationManager.FindByClientIdAsync("example-BE-client") is null)
            await applicationManager.CreateAsync(beClient);

        var managementPortal = new OpenIddictApplicationDescriptor
        {
            ClientId = "management-portal",
            ClientSecret = "management-portal-secret",
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            RedirectUris =
            {
                new Uri("https://localhost:5001/signin-oidc")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles
                }
        };

        if (await applicationManager.FindByClientIdAsync("management-portal") is null)
            await applicationManager.CreateAsync(managementPortal);
        
    }

    private static async Task CreateDefaultOidcScopes(AsyncServiceScope serviceScope)
    {
        var scopeManager = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        var defaultScopes = new[]
        {
            OpenIddictConstants.Scopes.Address,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.OfflineAccess,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Phone,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles
        };
        
        foreach (var defaultScope in defaultScopes)
        {
            if (await scopeManager.FindByNameAsync(defaultScope) is null)
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = defaultScope
                });
        }
        
        var testingScope = "BE1";
        if (await scopeManager.FindByNameAsync(testingScope) is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = testingScope,
                Resources =
                {
                    "example-BE-client"
                }
            });
        }
    }
    
    
}