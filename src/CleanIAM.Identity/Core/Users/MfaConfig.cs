namespace CleanIAM.Identity.Core.Users;

/// <summary>
/// Represents the configuration for multifactor authentication (MFA) for a user.
/// </summary>
public class MfaConfig
{
    /// <summary>
    /// Indicates whether the MFA is configured for the user.
    /// </summary>
    public bool IsMfaConfigured { get; set; }

    /// <summary>
    /// The secret key used for generating MFA codes.
    /// </summary>
    public string TotpSecretKey { get; set; } = string.Empty;
}