namespace Identity.Core.Events;

public record EmailVerificationRequestSent(Guid Id, Guid UserId, string Email, string FirstName, string LastName);