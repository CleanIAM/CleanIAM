using CleanIAM.SharedKernel.Core;

namespace CleanIAM.Users.Api.Controllers.Models;

/// <summary>
/// Api user model
/// </summary>
public class ApiUserModel
{
    /// <summary>
    /// Id of the user
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email of the user
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Indicates whether the user's email has been verified.
    /// </summary>
    public bool EmailVerified { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// last name of the user
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// User roles
    /// </summary>
    public required UserRole[] Roles { get; set; }

    /// <summary>
    /// Indicates whether the user account is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Indicates whether the user has enabled multifactor authentication (MFA).
    /// </summary>
    public bool IsMFAEnabled { get; set; }

    /// <summary>
    /// Indicates whether the user has configured multifactor authentication (MFA).
    /// </summary>
    public bool IsMFAConfigured { get; set; }

    /// <summary>
    /// Indicates whether the user has a pending invite or the profile is already set up.
    /// </summary>
    public bool IsInvitePending { get; set; }
}