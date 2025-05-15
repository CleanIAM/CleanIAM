using System.Text.Json;
using Mapster;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace CleanIAM.Applications.Core;

/// <summary>
/// Abstraction over `OpenIddictEntityFrameworkCoreApplication[Guid]`
/// </summary>
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
    public required string ClientId { get; set; }

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
    /// Allowed scopes for the application.
    /// </summary>
    public HashSet<string> Scopes { get; set; } = [];

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

    public OpenIddictApplicationDescriptor ToDescriptor()
    {
        return this.Adapt<OpenIddictApplicationDescriptor>();
    }

    public static async Task<OpenIdApplication> FromOpenIdDictApplication(
        OpenIddictEntityFrameworkCoreApplication<Guid> application,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager)
    {
        var permissions = await applicationManager.GetPermissionsAsync(application, CancellationToken.None);

        var dest = new OpenIdApplication
        {
            Id = application.Id,


            ApplicationType = application.ApplicationType switch
            {
                OpenIddictConstants.ApplicationTypes.Native => Core.ApplicationType.Native,
                OpenIddictConstants.ApplicationTypes.Web => Core.ApplicationType.Web,
                _ => null
            },

            ClientId = application.ClientId ?? string.Empty,
            ClientType = application.ClientType switch
            {
                OpenIddictConstants.ClientTypes.Public => Core.ClientType.Public,
                OpenIddictConstants.ClientTypes.Confidential => Core.ClientType.Confidential,
                var value => throw new InvalidDataException($"Invalid client type: {value}")
            },
            ConsentType = application.ConsentType switch
            {
                OpenIddictConstants.ConsentTypes.Implicit => Core.ConsentType.Implicit,
                OpenIddictConstants.ConsentTypes.Explicit => Core.ConsentType.Explicit,
                _ => null
            },
            DisplayName = application.DisplayName,

            PostLogoutRedirectUris =
            [
                ..(
                    await applicationManager.GetPostLogoutRedirectUrisAsync(application, CancellationToken.None))
                .Select(uri => new Uri(uri))
            ],
            RedirectUris =
            [
                ..(
                    await applicationManager.GetRedirectUrisAsync(application, CancellationToken.None))
                .Select(uri => new Uri(uri))
            ],
            Properties =
                new Dictionary<string, JsonElement>(
                    await applicationManager.GetPropertiesAsync(application, CancellationToken.None)),

            Scopes =
            [
                ..permissions
                    .Where(permission => permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope,
                        StringComparison.OrdinalIgnoreCase))
                    .Select(permission => permission[OpenIddictConstants.Permissions.Prefixes.Scope.Length..])
            ]
        };
        return dest;
    }
}