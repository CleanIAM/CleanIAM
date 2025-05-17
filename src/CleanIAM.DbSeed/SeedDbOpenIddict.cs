using Mapster;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using CleanIAM.Scopes;
using CleanIAM.SharedKernel.Core.Database;
using OpenIddictScope = OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreScope<System.Guid>;

namespace DbConfig;

public static class SeedDbOpenIddict
{

    public static async Task SeedOpenIddictObjects(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();

        await CreateManagementPortalOidcClient(scope);
        await CreateDefaultOidcScopes(scope);
    }

    private static async Task CreateManagementPortalOidcClient(AsyncServiceScope scope)
    {
        var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var managementConsoleFrontend = new OpenIddictApplicationDescriptor
        {
            ClientId = "management-portal",
            DisplayName = "Management Portal",
            ApplicationType = OpenIddictConstants.ApplicationTypes.Web,
            ClientType = OpenIddictConstants.ClientTypes.Public,
            RedirectUris =
            {
                new Uri("https://localhost:3001/auth/signin-callback")
            },
            PostLogoutRedirectUris = { new Uri("https://localhost:3001") },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Roles,
            },
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            }
        };

        if (await applicationManager.FindByClientIdAsync("management-portal") is null)
            await applicationManager.CreateAsync(managementConsoleFrontend);
    }

    private static async Task CreateDefaultOidcScopes(AsyncServiceScope scope)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<OpenIddictScopeManager<OpenIddictScope>>();

        var defaultScopes = ScopesConstants.DefaultScopes;

        foreach (var defaultScope in defaultScopes)
            if (await scopeManager.FindByNameAsync(defaultScope.Name) is null)
                await scopeManager.CreateAsync(defaultScope.Adapt<OpenIddictScope>());

    }
}