using SharedKernel.Core;

namespace Events.Core.Events.Users;

/// <summary>
/// Event that is published when a user is deleted.
/// </summary>
/// <param name="Id">Id of the user</param>
/// <param name="Email">Email of the user</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
/// <param name="Roles">Roles of the user</param>
public record UserDeleted(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);