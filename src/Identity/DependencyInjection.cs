using System.Text;
using CommunityToolkit.Diagnostics;
using Identity.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using JasperFx.CodeGeneration;
using Marten;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Trace;
using SharedKernel.Core.Database;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenIddict(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var encryptionKey = configuration.GetSection("OpenIddict")["EncryptionKey"];
        Guard.IsNotNullOrEmpty(encryptionKey, "Encryption key");

        serviceCollection.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize")
                    .SetTokenEndpointUris("/connect/token")
                    .SetIntrospectionEndpointUris("connect/introspect")
                    .SetEndSessionEndpointUris("/connect/endsession");

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
                    .EnableErrorPassthrough()
                    .EnableEndSessionEndpointPassthrough();

                options.DisableAccessTokenEncryption();

                options.AddEncryptionKey(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(encryptionKey)));
            });
        return serviceCollection;
    }

    public static async Task ConfigureOpenIddict(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();

        await CreateExampleOidcClient(scope);
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

        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync("BE1") is null)
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "BE1",
                Resources =
                {
                    "example-BE-client"
                }
            });

        if (await scopeManager.FindByNameAsync("api2") is null)
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "api2",
                Resources =
                {
                    "resource_server_2"
                }
            });
    }
}