using ManagementPortal.Application.Commands.OpenIdApplications;
using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace ManagementPortal;

public static class MapsterConfig
{
    public static void Configure()
    {
        // Configure special properties mapping for OpenIddict application objects
        // This special mappings uses IOpenIddictApplicationManager to use its implicit cache for parsing value strings
        TypeAdapterConfig<OpenIddictEntityFrameworkCoreApplication<Guid>, OpenIdApplication>.ForType()
            .AfterMapping((src, dest) =>
                throw new InvalidOperationException(
                    "This mapping should not be used. Use OpenIdApplication.From instead."));

        TypeAdapterConfig.GlobalSettings.Default
            .PreserveReference(true);

        TypeAdapterConfig<OpenIdApplication, OpenIddictApplicationDescriptor>.ForType()
            .AfterMapping((src, dest, context) =>
            {
                dest.Permissions.UnionWith(src.Scopes.Select(scope =>
                    OpenIddictConstants.Permissions.Prefixes.Scope + scope));
                dest.Permissions.UnionWith(src.Endpoints.Select(endpoint =>
                    OpenIddictConstants.Permissions.Prefixes.Endpoint + endpoint));
                dest.Permissions.UnionWith(src.GrantTypes.Select(grantType =>
                    OpenIddictConstants.Permissions.Prefixes.GrantType + grantType));
                dest.Permissions.UnionWith(src.ResponseTypes.Select(responseType =>
                    OpenIddictConstants.Permissions.Prefixes.ResponseType + responseType));
                dest.RedirectUris.UnionWith(src.RedirectUris);
                dest.PostLogoutRedirectUris.UnionWith(src.PostLogoutRedirectUris);
                dest.Requirements.UnionWith(src.Requirements);
            });

        TypeAdapterConfig<UpdateOpenIdApplicationCommand, OpenIddictApplicationDescriptor>.ForType()
            .AfterMapping((src, dest) =>
            {
                dest.Permissions.UnionWith(src.Scopes.Select(scope =>
                    OpenIddictConstants.Permissions.Prefixes.Scope + scope));
                dest.Permissions.UnionWith(src.Endpoints.Select(endpoint =>
                    OpenIddictConstants.Permissions.Prefixes.Endpoint + endpoint));
                dest.Permissions.UnionWith(src.GrantTypes.Select(grantType =>
                    OpenIddictConstants.Permissions.Prefixes.GrantType + grantType));
                dest.Permissions.UnionWith(src.ResponseTypes.Select(responseType =>
                    OpenIddictConstants.Permissions.Prefixes.ResponseType + responseType));

                dest.RedirectUris.UnionWith(src.RedirectUris);
                dest.PostLogoutRedirectUris.UnionWith(src.PostLogoutRedirectUris);
                dest.Requirements.UnionWith(src.Requirements);
            });

        TypeAdapterConfig<CreateNewOpenIdApplicationCommand, OpenIddictApplicationDescriptor>.ForType()
            .AfterMapping((src, dest) =>
            {
                dest.Permissions.UnionWith(src.Scopes.Select(scope =>
                    OpenIddictConstants.Permissions.Prefixes.Scope + scope));
                dest.Permissions.UnionWith(src.Endpoints.Select(endpoint =>
                    OpenIddictConstants.Permissions.Prefixes.Endpoint + endpoint));
                dest.Permissions.UnionWith(src.GrantTypes.Select(grantType =>
                    OpenIddictConstants.Permissions.Prefixes.GrantType + grantType));
                dest.Permissions.UnionWith(src.ResponseTypes.Select(responseType =>
                    OpenIddictConstants.Permissions.Prefixes.ResponseType + responseType));

                dest.RedirectUris.UnionWith(src.RedirectUris);
                dest.PostLogoutRedirectUris.UnionWith(src.PostLogoutRedirectUris);
                dest.Requirements.UnionWith(src.Requirements);
            });
    }
}