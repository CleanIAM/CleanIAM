using SharedKernel.Core;

namespace ManagementPortal.Api.Views.Users;

public class UserFormModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole[] Roles { get; set; } = Array.Empty<UserRole>();
    public bool IsEditMode { get; set; }
}