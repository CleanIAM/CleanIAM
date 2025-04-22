using System.Globalization;
using System.Text.Json;
using Mapster;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace ManagementPortal.Core.OpenIdApplication;

public class OpenIdApplication
{
    /// <summary>
    /// Gets or sets the id associated with the application.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the application type associated with the application.
    /// </summary>
    public ApplicationType? ApplicationType { get; set; }

    /// <summary>
    /// Gets or sets the client identifier associated with the application.
    /// </summary>
    public string ClientId { get; set; }

    // <summary>
    // Gets or sets the client secret associated with the application.
    // Note: depending on the application manager used when creating it,
    // this property may be hashed or encrypted for security reasons.
    // </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the client type associated with the application.
    /// </summary>
    public ClientType? ClientType { get; set; }

    /// <summary>
    /// Gets or sets the consent type associated with the application.
    /// </summary>
    public ConsentType? ConsentType { get; set; }

    /// <summary>
    /// Gets or sets the display name associated with the application.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets the localized display names associated with the application.
    /// </summary>
    public Dictionary<CultureInfo, string> DisplayNames { get; set; } = [];

    /// <summary>
    /// Allowed scopes for the application.
    /// </summary>
    public HashSet<string> Scopes { get; set; } = [];
    
    /// <summary>
    /// Allowed endpoints for the application.
    /// </summary>
    public HashSet<string> Endpoints { get; set; } = [];
    
    /// <summary>
    /// Allowed grant types for the application.
    /// </summary>
    public HashSet<string> GrantTypes { get; set; } = [];
    
    /// <summary>
    /// Allowed response types for the application.
    /// </summary>
    public HashSet<string> ResponseTypes { get; set; } = [];

    /// <summary>
    /// Gets the post-logout redirect URIs associated with the application.
    /// </summary>
    public HashSet<Uri> PostLogoutRedirectUris { get; set; } = [];

    /// <summary>
    /// Gets the additional properties associated with the application.
    /// </summary>
    public Dictionary<string, JsonElement> Properties { get; set; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the redirect URIs associated with the application.
    /// </summary>
    public HashSet<Uri> RedirectUris { get; set; } = [];

    /// <summary>
    /// Gets the requirements associated with the application.
    /// </summary>
    public HashSet<string> Requirements { get; set; } = new(StringComparer.Ordinal);
    
    
    public OpenIddictApplicationDescriptor ToDescriptor()
    {
        return this.Adapt<OpenIddictApplicationDescriptor>();
    }
    
    public static async Task<OpenIdApplication> FromOpenIdDictApplication(OpenIddictEntityFrameworkCoreApplication<Guid> application,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager)
    {
        var dest = application.Adapt<OpenIdApplication>();
        dest.PostLogoutRedirectUris = new((
                await applicationManager.GetPostLogoutRedirectUrisAsync(application, CancellationToken.None))
            .Select(uri => new Uri(uri)));
        dest.RedirectUris = new((
                await applicationManager.GetRedirectUrisAsync(application, CancellationToken.None))
            .Select(uri => new Uri(uri)));
        dest.Requirements = new(await applicationManager.GetRequirementsAsync(application, CancellationToken.None));
        dest.Properties = new(await applicationManager.GetPropertiesAsync(application, CancellationToken.None));
        dest.DisplayNames = new(await applicationManager.GetDisplayNamesAsync(application, CancellationToken.None));

        
        var permissionsRaw = await applicationManager.GetPermissionsAsync(application, CancellationToken.None);
        dest.Scopes = new(permissionsRaw
            .Where(permission => permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope, StringComparison.OrdinalIgnoreCase))
            .Select(permission => permission[OpenIddictConstants.Permissions.Prefixes.Scope.Length..]));
        
        dest.Endpoints = new(permissionsRaw
            .Where(permission => permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.Endpoint, StringComparison.OrdinalIgnoreCase))
            .Select(permission => permission[OpenIddictConstants.Permissions.Prefixes.Endpoint.Length..]));
        
        dest.GrantTypes = new(permissionsRaw
            .Where(permission => permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType, StringComparison.OrdinalIgnoreCase))
            .Select(permission => permission[OpenIddictConstants.Permissions.Prefixes.GrantType.Length..]));
        
        dest.ResponseTypes = new(permissionsRaw
            .Where(permission => permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.ResponseType, StringComparison.OrdinalIgnoreCase))
            .Select(permission => permission[OpenIddictConstants.Permissions.Prefixes.ResponseType.Length..]))
;
        
        return dest;
    }
}