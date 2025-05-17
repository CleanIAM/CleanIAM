using CleanIAM.SharedKernel.Core.Database;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddictScope = OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreScope<System.Guid>;


namespace DbConfig;

public static  class SeedDemoAppsWrapper
    {

        public static async Task SeedDemoApps(this IApplicationBuilder app)
        {
            await using var scope = app.ApplicationServices.CreateAsyncScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            await CreateDemoOidcClient(scope);
            await CreateDemoOidcScopes(scope);
        }

        private static async Task CreateDemoOidcClient(AsyncServiceScope scope)
        {
            var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            /* DEMO APPLICATIONS */
            var feClient = new OpenIddictApplicationDescriptor
            {
                ClientId = "example-FE-client",
                DisplayName = "Example FE Client",
                ApplicationType = OpenIddictConstants.ApplicationTypes.Web,
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
                    OpenIddictConstants.Permissions.Endpoints.EndSession,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Scopes.OpenId,
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
                DisplayName = "Example BE Client",
                ClientSecret = "test-secret",
                ApplicationType = OpenIddictConstants.ApplicationTypes.Native,
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Introspection
                }
            };

            if (await applicationManager.FindByClientIdAsync("example-BE-client") is null)
                await applicationManager.CreateAsync(beClient);
        }

        private static async Task CreateDemoOidcScopes(AsyncServiceScope scope)
        {
            var scopeManager = scope.ServiceProvider.GetRequiredService<OpenIddictScopeManager<OpenIddictScope>>();

            var testingScope = "BE1";
            if (await scopeManager.FindByNameAsync(testingScope) is null)
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = testingScope,
                    Description = "Testing scope",
                    DisplayName = "Testing scope",
                    Resources =
                    {
                        "example-BE-client"
                    }
                });
        }
}