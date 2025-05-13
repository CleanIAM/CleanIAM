using CleanIAM.SharedKernel.Core;

namespace CleanIAM.Users.Core.Events.Users;

public record UserDeleted(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);