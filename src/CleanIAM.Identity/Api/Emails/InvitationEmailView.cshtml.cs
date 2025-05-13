using CleanIAM.Identity.Core.Mails;

namespace CleanIAM.Identity.Api.Emails;

/// <summary>
/// View model for the password reset email
/// </summary>
public class InvitationEmailViewModel
{
    /// <summary>
    /// Recipient of the email
    /// </summary>
    public required EmailRecipient Recipient { get; set; }

    /// <summary>
    /// URL used setup users account
    /// </summary>
    public required string InvitationUrl { get; set; }
}