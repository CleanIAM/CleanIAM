using SharedKernel.Core;

namespace ManagementPortal.Api.Controllers.Models;


/// <summary>
/// Api user model
/// </summary>
public class ApiUserModel
{
    /// <summary>
    /// Id of the user
    /// </summary>
    public Guid Id { get; set; }
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