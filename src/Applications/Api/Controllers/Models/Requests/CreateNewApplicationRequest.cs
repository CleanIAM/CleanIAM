using System.ComponentModel.DataAnnotations;
using Applications.Core;

namespace Applications.Api.Controllers.Models.Requests;

public class CreateNewApplicationRequest
{
    /// <summary>
    /// The client identifier associated with the application.
    /// </summary>
    [Required]
    [Length(1, 32, ErrorMessage = "ClientId length must be between 1 and 32 character long")]
    [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
    public required string ClientId { get; set; }

    /// <summary>
    /// The display name associated with the application.
    /// </summary>
    [Required]
    [Length(1, 32, ErrorMessage = "Display name length must be between 1 and 32 character long")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// The application type associated with the application.
    /// </summary>
    [Required]
    public ApplicationType? ApplicationType { get; set; }

    /// <summary>
    /// The client type associated with the application.
    /// </summary>
    [Required]
    public ClientType? ClientType { get; set; }

    /// <summary>
    /// The consent type associated with the application.
    /// </summary>
    [Required]
    public ConsentType? ConsentType { get; set; }

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