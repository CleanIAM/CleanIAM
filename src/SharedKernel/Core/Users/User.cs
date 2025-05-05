namespace SharedKernel.Core.Users;

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
    /// Represents the hashed password of the user, including its hash, salt, and hash algorithm signature.
    /// </summary>
    public HashedPassword HashedPassword { get; set; }

    /// <summary>
    /// Indicates whether the user has enabled multi-factor authentication (MFA).
    /// </summary>
    public bool IsMFAEnabled { get; set; }

    /// <summary>
    /// Indicates whether the user account is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }
}