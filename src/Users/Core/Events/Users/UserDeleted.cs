using SharedKernel.Core;

namespace Users.Core.Events.Users;

public record UserDeleted(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);