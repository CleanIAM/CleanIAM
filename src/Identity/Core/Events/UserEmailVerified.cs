namespace Identity.Core.Events;

/// <summary>
/// Event that is published when a user verifies its email.
/// </summary>
/// <param name="Id">User Id</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
/// <param name="Email">Email of the user</param>
public record UserEmailVerified(Guid Id, string FirstName, string LastName, string Email);