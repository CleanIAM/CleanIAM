namespace Events.Core.Events.ManagementPortal.Mfa;

/// <summary>
/// Represents the event of configuring multifactor authentication (MFA) for a user.
/// </summary>
/// <param name="Id">User id</param>
/// <param name="TotpSecretKey">New TotpSecretKey</param>
/// <param name="MfaEnabled">Is Mfa enabled for user</param>
public record MfaConfiguredForUser(Guid Id, string TotpSecretKey, bool MfaEnabled);