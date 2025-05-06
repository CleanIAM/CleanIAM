using SharedKernel.Core;

namespace ManagementPortal.Api.Controllers.Models.Requests.Users;

/// <summary>
/// Request model for inviting a user.
/// </summary>
public class InviteUserRequest
{
    /// <summary>
    /// Email of the invited user
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// First name of the invited user
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Last name of the invited user
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Roles of the invited user
    /// </summary>
    public required UserRole[] Roles { get; set; }
}