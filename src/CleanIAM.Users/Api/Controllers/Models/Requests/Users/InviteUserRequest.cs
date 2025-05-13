using System.ComponentModel.DataAnnotations;
using CleanIAM.SharedKernel.Core;

namespace CleanIAM.Users.Api.Controllers.Models.Requests.Users;

/// <summary>
/// Request model for inviting a user.
/// </summary>
public class InviteUserRequest
{
    /// <summary>
    /// Email of the invited user
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// First name of the invited user
    /// </summary>
    [Required]
    [Length(2, 64, ErrorMessage = "First name length must be between 2 and 64 character long")] 
    public required string FirstName { get; set; }

    /// <summary>
    /// Last name of the invited user
    /// </summary>
    [Required]
    [Length(2, 64, ErrorMessage = "First name length must be between 2 and 64 character long")]
    public required string LastName { get; set; }

    /// <summary>
    /// Roles of the invited user
    /// </summary>
    [Required]
    public required UserRole[] Roles { get; set; }
}