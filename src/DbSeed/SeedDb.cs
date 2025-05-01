using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using SharedKernel.Core.Database;

namespace DbConfig;

public static class SeedDb
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        dbContext.Database.EnsureCreated();
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