namespace Events.Core.Events.Identity;

/// <summary>
/// UserLoggedIn event.
/// </summary>
/// <param name="Id">id of the user</param>
/// <param name="FirstName">FirstName of the user</param>
/// <param name="LastName">LastName of the user</param>
/// <param name="Email">Email of the user</param>
public record UserLoggedIn(Guid Id, string FirstName, string LastName, string Email);