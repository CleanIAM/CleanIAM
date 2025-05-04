using System.ComponentModel.DataAnnotations;
using ManagementPortal.Core.OpenIdApplication;

namespace ManagementPortal.Api.Controllers.Models.Requests;

public class CreateNewApplicationRequest
{
    /// <summary>
    /// Gets or sets the application type associated with the application.
    /// </summary>
    public ApplicationType? ApplicationType { get; set; }

    /// <summary>
    /// Gets or sets the client identifier associated with the application.
    /// </summary>
    [Required]
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
    [Required]
    public HashSet<string> Scopes { get; set; } = [];

    /// <summary>
    /// Gets the post-logout redirect URIs associated with the application.
    /// </summary>
    [Required]
    public HashSet<Uri> PostLogoutRedirectUris { get; set; } = [];

    /// <summary>
    /// Gets the redirect URIs associated with the application.
    /// </summary>
    [Required]
    public HashSet<Uri> RedirectUris { get; set; } = [];
}