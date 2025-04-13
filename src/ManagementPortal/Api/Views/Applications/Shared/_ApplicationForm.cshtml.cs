using System.Globalization;
using System.Text.Json;
using ManagementPortal.Core.OpenIdApplication;
using Microsoft.IdentityModel.Tokens;

namespace ManagementPortal.Api.Views.Applications.Shared;

public class ApplicationFormModel
{
    /// <summary>
    /// Gets or sets whether this form is in edit mode
    /// </summary>
    public bool IsEditMode { get; set; }
    
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
    /// Gets or sets the JSON Web Key Set associated with the application.
    /// </summary>
    public JsonWebKeySet? JsonWebKeySet { get; set; }

    /// <summary>
    /// Gets the permissions associated with the application.
    /// </summary>
    public HashSet<string> Permissions { get; set; } = new(StringComparer.Ordinal);

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

    /// <summary>
    /// Gets the settings associated with the application.
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new(StringComparer.Ordinal);
}