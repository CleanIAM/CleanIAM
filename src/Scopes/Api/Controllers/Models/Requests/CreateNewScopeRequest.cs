using System.ComponentModel.DataAnnotations;

namespace Scopes.Api.Controllers.Models.Requests;

/// <summary>
/// Request to create a new scope.
/// </summary>
public class CreateNewScopeRequest
{
    /// <summary>
    /// The name of the scope.
    /// </summary>
    [Required]
    [Length(3, 32, ErrorMessage = "Name length must be between 3 and 32 character long")]
    [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
    public required string Name { get; set; }

    /// <summary>
    /// The display name of the scope.
    /// </summary>
    [Required]
    [Length(3, 32, ErrorMessage = "Display name length must be between 3 and 32 character long")]
    public required string DisplayName { get; set; }

    /// <summary>
    /// The description of the scope.
    /// </summary>
    [MaxLength(256, ErrorMessage = "Description cannot be longer than 256 character long")]
    public string? Description { get; set; }

    /// <summary>
    /// The resources the scopes allows access to.
    /// </summary>
    [Required]
    public required string[] Resources { get; set; }
}