using System.ComponentModel.DataAnnotations;
using CleanIAM.Applications.Core;

namespace CleanIAM.Applications.Api.Controllers.Models;

public class ApiApplicationModel
{
    /// <summary>
    /// The id associated with the application.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The application type associated with the application.
    /// </summary>
    public ApplicationType? ApplicationType { get; set; }

    /// <summary>
    /// The client identifier associated with the application.
    /// </summary>
    [Required]
    public string ClientId { get; set; }

    /// <summary>
    /// The client type associated with the application.
    /// </summary>
    public ClientType? ClientType { get; set; }

    /// <summary>
    /// The consent type associated with the application.
    /// </summary>
    public ConsentType? ConsentType { get; set; }

    /// <summary>
    /// The display name associated with the application.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Allowed scopes for the application.
    /// </summary>
    [Required]
    public HashSet<string> Scopes { get; set; } = [];

    /// <summary>
    /// Post-logout redirect URIs associated with the application.
    /// </summary>
    [Required]
    public HashSet<Uri> PostLogoutRedirectUris { get; set; } = [];

    /// <summary>
    /// Redirect URIs associated with the application.
    /// </summary>
    [Required]
    public HashSet<Uri> RedirectUris { get; set; } = [];
}