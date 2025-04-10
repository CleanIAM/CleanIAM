using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace ManagementPortal;

public static class MapsterConfig
{
    public static void Configure()
    {
            
        // Configure special properties mapping for OpenIddict application objects
        // This special mappings uses IOpenIddictApplicationManager to use its implicit cache for parsing value strings
        TypeAdapterConfig<OpenIddictEntityFrameworkCoreApplication<Guid>, OpenIdApplication>.ForType()
            .IgnoreNullValues(true)
            .AfterMappingAsync(async (src, dest) =>
            {
                var applicationManager = MapContext.Current.GetService<OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>>>();
                
                // dest.Permissions = new(await applicationManager.GetPermissionsAsync(src, CancellationToken.None));
                // dest.PostLogoutRedirectUris = new((
                //     await applicationManager.GetPostLogoutRedirectUrisAsync(src, CancellationToken.None))
                //     .Select(uri => new Uri(uri)));
                // dest.RedirectUris = new((
                //     await applicationManager.GetRedirectUrisAsync(src, CancellationToken.None))
                //     .Select(uri => new Uri(uri)));
                // dest.Requirements = new(await applicationManager.GetRequirementsAsync(src, CancellationToken.None));
                // dest.Properties = new(await applicationManager.GetPropertiesAsync(src, CancellationToken.None));
                // dest.Settings = new(await applicationManager.GetSettingsAsync(src, CancellationToken.None));
                // dest.DisplayNames = new(await applicationManager.GetDisplayNamesAsync(src, CancellationToken.None));
                // dest.JsonWebKeySet = await applicationManager.GetJsonWebKeySetAsync(src, CancellationToken.None);
                
            });
        
        
    }
}