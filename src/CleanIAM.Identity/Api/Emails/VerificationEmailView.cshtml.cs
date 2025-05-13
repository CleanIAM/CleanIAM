using CleanIAM.Identity.Core.Mails;

namespace CleanIAM.Identity.Api.Emails;

/// <summary>
/// View model for the verification email.
/// </summary>
public class VerificationEmailViewModel
{
    /// <summary>
    /// Recipient of the email
    /// </summary>
    public required EmailRecipient Recipient { get; set; }

    /// <summary>
    /// URL used to verify the user's email address.
    /// </summary>
    public required string VerificationUrl { get; set; }
}