using CleanIAM.Identity.Core.Mails;

namespace CleanIAM.Identity.Api.Emails;

/// <summary>
/// View model for the password reset email
/// </summary>
public class PasswordResetEmailViewModel
{
    /// <summary>
    /// Recipient of the email
    /// </summary>
    public required EmailRecipient Recipient { get; set; }

    /// <summary>
    /// URL used to reset the user's password
    /// </summary>
    public required string PasswordResetUrl { get; set; }
}