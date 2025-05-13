using CleanIAM.SharedKernel.Core;

namespace CleanIAM.Events.Core.Events.Users;

/// <summary>
/// Event that is published when a user is updated.
/// </summary>
/// <param name="Id">Id of the user</param>
/// <param name="Email">Email of the user</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
/// <param name="Roles">Roles of the user</param>
public record UserUpdated(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);