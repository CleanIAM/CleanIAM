using SharedKernel.Core;

namespace ManagementPortal.Core;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public UserRole[] Roles { get; set; } 
}