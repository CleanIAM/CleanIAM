namespace Identity.Core.Events;

/// <summary>
/// Event that is published when a user requests a password reset.
/// </summary>
/// <param name="Id">Id of the password reset request</param>
/// <param name="UserId">Id of the user</param>
/// <param name="Email">Email of the user</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
public record PasswordResetRequestSent(Guid Id, Guid UserId, string Email, string FirstName, string LastName);