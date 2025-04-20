using System.ComponentModel.DataAnnotations;
using SharedKernel.Core;

namespace ManagementPortal.Api.Views.Users;

public class CreateNewUserModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    public UserRole[] Roles { get; set; } = new[] { UserRole.User };
}