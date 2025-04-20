using SharedKernel.Core;

namespace ManagementPortal.Core.Events.Users;

public record UserCreated(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);