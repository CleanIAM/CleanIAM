namespace Events.Core.Events.Identity;

/// <summary>
/// Event that is published when a user resets its password.
/// </summary>
/// <param name="Id">User Id</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
/// <param name="Email">Email of the user</param>
public record PasswordReset(Guid Id, string FirstName, string LastName, string Email);