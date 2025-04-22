using System.Globalization;
using System.Text.Json;
using ManagementPortal.Core.OpenIdApplication;
using Microsoft.IdentityModel.Tokens;
namespace ManagementPortal.Api.Views.Applications;

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
    public ApplicationType? ApplicationType { get; set; } = Core.OpenIdApplication.ApplicationType.Web;

    /// <summary>
    /// Gets or sets the client identifier associated with the application.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client type associated with the application.
    /// </summary>
    public ClientType? ClientType { get; set; } = Core.OpenIdApplication.ClientType.Public;

    /// <summary>
    /// Gets or sets the consent type associated with the application.
    /// </summary>
    public ConsentType? ConsentType { get; set; } = Core.OpenIdApplication.ConsentType.Explicit;

    /// <summary>
    /// Gets or sets the display name associated with the application.
    /// </summary>
    public string? DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the localized display names associated with the application.
    /// </summary>
    public Dictionary<CultureInfo, string> DisplayNames { get; set; } = [];
    
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
    
    
    public HashSet<string> Scopes { get; set; } = [];
    public HashSet<string> Endpoints { get; set; } = [];
    public HashSet<string> GrantTypes { get; set; } = [];
    public HashSet<string> ResponseTypes { get; set; } = [];
}