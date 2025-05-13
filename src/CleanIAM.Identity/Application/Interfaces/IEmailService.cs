using CleanIAM.Identity.Core.Mails;
using CleanIAM.SharedKernel.Infrastructure.Utils;

namespace CleanIAM.Identity.Application.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Sends a verification email to the user.
    /// </summary>
    /// <param name="recipient">Info about email recipient</param>
    /// <param name="verificationUrl">link for email verification to embed into the email</param>
    Task<Result> SendVerificationEmailAsync(EmailRecipient recipient, string verificationUrl);

    /// <summary>
    /// Sends a password-reset email to the user.
    /// </summary>
    /// <param name="recipient">Info about email recipient</param>
    /// <param name="passwordResetUrl">Link for the password reset to embed into the email</param>
    Task<Result> SendPasswordResetEmailAsync(EmailRecipient recipient, string passwordResetUrl);

    /// <summary>
    /// Sends a invitaion email to the user.
    /// </summary>
    /// <param name="recipient">Info about email recipient</param>
    /// <param name="invitationUrl">Link for the account setup to embed into the mail</param>
    Task<Result> SendInvitationEmailAsync(EmailRecipient recipient, string invitationUrl);
}