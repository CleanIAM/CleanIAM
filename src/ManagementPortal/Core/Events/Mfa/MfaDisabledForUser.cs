namespace ManagementPortal.Core.Events.Mfa;

/// <summary>
/// Represents the event of user disabling multifactor authentication (MFA).
/// </summary>
/// <param name="Id">Id of user</param>
public record MfaDisabledForUser(Guid Id);