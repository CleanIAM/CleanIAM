using SharedKernel.Core;

namespace Identity.Core.Users;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserRole[] Roles { get; set; } 
    public HashedPassword HashedPassword { get; set; }
}