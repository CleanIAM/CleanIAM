using ManagementPortal.Api.Controllers.Models;
using ManagementPortal.Application.Commands.OpenIdApplications;
using ManagementPortal.Core.OpenIdApplication;
using ManagementPortal.Core.Users;
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
            .BeforeMapping((_, _) =>
                throw new InvalidOperationException(
                    "This mapping should not be used. Use OpenIdApplication.From instead."));

        TypeAdapterConfig.GlobalSettings.Default
            .PreserveReference(true);

        TypeAdapterConfig<OpenIdApplication, OpenIddictApplicationDescriptor>.ForType()
            .AfterMapping((src, dest, context) =>
            {
                dest.Permissions.UnionWith(src.Scopes.Select(scope =>
                    OpenIddictConstants.Permissions.Prefixes.Scope + scope));
                dest.RedirectUris.UnionWith(src.RedirectUris);
                dest.PostLogoutRedirectUris.UnionWith(src.PostLogoutRedirectUris);
            });

        TypeAdapterConfig<UpdateOpenIdApplicationCommand, OpenIddictApplicationDescriptor>.ForType()
            .AfterMapping((src, dest) =>
            {
                // Normalize values to lowercase to avoid case-sensitive issues in OpenIddict
                dest.ApplicationType = dest.ApplicationType?.ToLower();
                dest.ClientType = dest.ClientType?.ToLower();
                dest.ConsentType = dest.ConsentType?.ToLower();

                dest.Permissions.UnionWith(src.Scopes.Select(scope =>
                    OpenIddictConstants.Permissions.Prefixes.Scope + scope));

                dest.RedirectUris.UnionWith(src.RedirectUris);
                dest.PostLogoutRedirectUris.UnionWith(src.PostLogoutRedirectUris);
            });

        TypeAdapterConfig<CreateNewOpenIdApplicationCommand, OpenIddictApplicationDescriptor>.ForType()
            .AfterMapping((src, dest) =>
            {
                // Normalize values to lowercase to avoid case-sensitive issues in OpenIddict
                dest.ApplicationType = dest.ApplicationType?.ToLower();
                dest.ClientType = dest.ClientType?.ToLower();
                dest.ConsentType = dest.ConsentType?.ToLower();

                dest.Permissions.UnionWith(src.Scopes.Select(scope =>
                    OpenIddictConstants.Permissions.Prefixes.Scope + scope));

                dest.RedirectUris.UnionWith(src.RedirectUris);
                dest.PostLogoutRedirectUris.UnionWith(src.PostLogoutRedirectUris);
            });

        TypeAdapterConfig<User, ApiUserModel>.ForType()
            .Map(dest => dest.IsMFAConfigured, src => src.MfaConfig.IsMfaConfigured);
    }
}