using SharedKernel.Core;

namespace Users.Core.Events.Users;

/// <summary>
/// Represents an event that is triggered when a user is invited.
/// </summary>
/// <param name="Id">Id of the invited user</param>
/// <param name="Email">Email of the invited user</param>
/// <param name="FirstName">First name of the invited user</param>
/// <param name="LastName">Last name of the invited user</param>
/// <param name="Roles">Roles of the invited user</param>
public record UserInvited(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);