using System.ComponentModel.DataAnnotations;
using CleanIAM.SharedKernel.Core;

namespace CleanIAM.Users.Api.Controllers.Models.Requests.Users;

public class UpdateUserRequest
{
    /// <summary>
    /// First name of the user
    /// </summary>
    [Required]
    [Length(2, 64, ErrorMessage = "First name length must be between 2 and 64 character long")] 
    public required string FirstName { get; set; }

    /// <summary>
    /// last name of the user
    /// </summary>
    [Required]
    [Length(2, 64, ErrorMessage = "First name length must be between 2 and 64 character long")]
    public required string LastName { get; set; }

    /// <summary>
    /// User roles
    /// </summary>
    [Required]
    public required UserRole[] Roles { get; set; }
}