namespace Events.Core.Events.ManagementPortal.Mfa;

/// <summary>
/// Represents the event of user enabling multifactor authentication (MFA).
/// </summary>
/// <param name="Id">Id of user</param>
public record MfaEnabledForUser(Guid Id);