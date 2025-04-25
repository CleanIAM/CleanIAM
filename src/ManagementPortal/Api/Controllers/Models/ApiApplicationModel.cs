using System.Globalization;
using ManagementPortal.Core.OpenIdApplication;

namespace ManagementPortal.Api.Controllers.Models;

public class ApiApplicationModel
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
    /// Gets the redirect URIs associated with the application.
    /// </summary>
    public HashSet<Uri> RedirectUris { get; set; } = [];

    /// <summary>
    /// Gets the requirements associated with the application.
    /// </summary>
    public HashSet<string> Requirements { get; set; } = new(StringComparer.Ordinal);
}