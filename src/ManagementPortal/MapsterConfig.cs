using ManagementPortal.Api.Views.Applications;
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
        .Ignore(dest => dest.Permissions,
            dest => dest.PostLogoutRedirectUris,
            dest => dest.RedirectUris,
            dest => dest.Requirements,
            dest => dest.Properties,
            dest => dest.Settings,
            dest => dest.DisplayNames,
            dest => dest.JsonWebKeySet
        );
    
    TypeAdapterConfig.GlobalSettings.Default
        .PreserveReference(true);

    TypeAdapterConfig<OpenIdApplication, OpenIddictApplicationDescriptor>.ForType()
        .AfterMapping((src, dest, context) =>
        {
            dest.Permissions.UnionWith(src.Permissions);
            dest.RedirectUris.UnionWith(src.RedirectUris);
            dest.PostLogoutRedirectUris.UnionWith(src.PostLogoutRedirectUris);
            dest.Requirements.UnionWith(src.Requirements);
        });


    TypeAdapterConfig<UpdateOpenIdApplicationCommand, OpenIddictApplicationDescriptor>.ForType()
        .AfterMapping((src, dest) =>
        {
            dest.Permissions.UnionWith(src.Permissions);
            dest.RedirectUris.UnionWith(src.RedirectUris);
            dest.PostLogoutRedirectUris.UnionWith(src.PostLogoutRedirectUris);
            dest.Requirements.UnionWith(src.Requirements);
        });

    }
}