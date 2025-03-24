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
using Identity.Core.Database;
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
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Http.Marten;
using Wolverine.Marten;

namespace Identity;

public static class DependencyInjection
{
    public static IHostBuilder UseProjects(this IHostBuilder host, string[] assemblies)
    {
        host.UseWolverine(opts =>
        {
            foreach (var assembly in assemblies)
                opts.Discovery.IncludeAssembly(Assembly.Load(assembly));
            opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;

            opts.Policies.AutoApplyTransactions();
            opts.Policies.UseDurableLocalQueues();
            opts.UseFluentValidation();
            opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
        });

        MapsterConfig.Configure();

        return host;
    }

    public static IServiceCollection AddDatabases(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetSection("DbSettings:ConnectionStrings")["oidc"];
        Guard.IsNotNullOrEmpty(dbConnectionString, "Connection string not provided");

        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(dbConnectionString);

            options.UseOpenIddict();
        });

        serviceCollection.AddMarten(configuration);

        return serviceCollection;
    }


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
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

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
                OpenIddictConstants.Permissions.Prefixes.Scope
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        };

        if (await manager.FindByClientIdAsync("example-FE-client") is null)
            await manager.CreateAsync(feClient);


        var beClient = new OpenIddictApplicationDescriptor
        {
            ClientId = "example-BE-client",
            ClientSecret = "test-secret",
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Permissions.Prefixes.Scope + "test-api"
            }
        };

        if (await manager.FindByClientIdAsync("example-BE-client") is null)
            await manager.CreateAsync(beClient);
    }

    public static IServiceCollection AddMarten(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSchemeName = configuration.GetSection("DbSettings:DatabaseNames")["MartenDb"];
        var connectionString =
            configuration.GetSection("DbSettings:ConnectionStrings")["MartenDb"];
        Guard.IsNotNullOrEmpty(dbSchemeName, "Db scheme");
        Guard.IsNotNullOrEmpty(connectionString, "Connection string");

        services.AddMarten(opts =>
            {
                opts.Connection(connectionString);
                opts.DatabaseSchemaName = dbSchemeName;
                opts.Policies.AllDocumentsAreMultiTenanted();
            })
            .ApplyAllDatabaseChangesOnStartup()
            .UseLightweightSessions()
            .IntegrateWithWolverine();

        // TODO: Add multitenancy support
        // services.AddMartenTenancyDetection(opts =>
        // {
        //     // Tenant name is organization id
        //     opts.IsClaimTypeNamed(SharedKernelConstants.OrganizationId);
        //     opts.DefaultIs("default_tenant");
        // });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        return services;
    }
}