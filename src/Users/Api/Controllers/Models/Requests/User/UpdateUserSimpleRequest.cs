using System.ComponentModel.DataAnnotations;

namespace Users.Api.Controllers.Models.Requests.User;

public class UpdateUserSimpleRequest
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
}