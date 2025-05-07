using SharedKernel.Core;

namespace ManagementPortal.Core.Users;

/// <summary>
/// The main user entity for the identity system.
/// </summary>
public class User
{
    /// <summary>
    /// Id of the user
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email address of the user
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Indicates whether the user's email has been verified.
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Last name of the user
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// List of roles assigned to the user.
    /// </summary>
    public UserRole[] Roles { get; set; }

    /// <summary>
    /// The id of the tenant to which the user belongs.
    /// </summary>
    public Guid TenantId { get; set; } = Guid.Empty;

    /// <summary>
    /// The name of the tenant to which the user belongs.
    /// </summary>
    public string TenantName { get; set; } = "Default Tenant";

    /// <summary>
    /// Indicates whether the user has enabled multifactor authentication (MFA).
    /// </summary>
    public bool IsMFAEnabled { get; set; }

    /// <summary>
    /// The configuration for multifactor authentication (MFA) for the user.
    /// </summary>
    public MfaConfig MfaConfig { get; set; } = new() { IsMfaConfigured = false, TotpSecretKey = "" };

    /// <summary>
    /// Indicates whether the user has a pending invite or the profile is already set up.
    /// </summary>
    public bool IsInvitePending { get; set; }

    /// <summary>
    /// Indicates whether the user account is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Sets the user as setup, indicating that the user has completed the setup process.
    /// </summary>
    public void SetUserAsSetup()
    {
        IsInvitePending = false;
    }

    /// <summary>
    /// Sets the user email as verified, indicating that the user has verified their email address.
    /// </summary>
    public void SetUserEmailAsVerified()
    {
        EmailVerified = true;
    }
}