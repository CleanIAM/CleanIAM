using Identity.Core.Mails;
using SharedKernel.Infrastructure;

namespace Identity.Application.Interfaces;

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
}