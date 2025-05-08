using SharedKernel.Core;

namespace Users.Api.Controllers.Models.Requests.Users;

public class CreateNewUserRequest
{
    /// <summary>
    /// Email of the user
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// last name of the user
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// User roles
    /// </summary>
    public required UserRole[] Roles { get; set; }
}