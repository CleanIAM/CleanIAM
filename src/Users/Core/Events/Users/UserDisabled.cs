namespace Users.Core.Events.Users;

/// <summary>
/// Represents an event triggered when a user has been disabled within the system.
/// </summary>
/// <param name="Id">Id of the user</param>
/// <param name="Email">Email address of the user</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
public record UserDisabled(Guid Id, string Email, string FirstName, string LastName);