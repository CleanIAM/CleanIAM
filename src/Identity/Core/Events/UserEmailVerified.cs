namespace Identity.Core.Events;

public record UserEmailVerified(Guid Id, string FirstName, string LastName, string Email);