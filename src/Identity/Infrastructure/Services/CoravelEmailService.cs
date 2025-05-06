using System.Net;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Interfaces;
using Identity.Api.Emails;
using Identity.Application.Interfaces;
using Identity.Core.Mails;
using SharedKernel.Infrastructure;

namespace Identity.Infrastructure.Services;

/// <summary>
/// </summary>
/// <param name="mailer"></param>
public class CoravelEmailService(IMailer mailer, ILogger<CoravelEmailService> logger) : IEmailService
{
    public async Task<Result> SendVerificationEmailAsync(EmailRecipient recipient, string verificationUrl)
    {
        try
        {
            await mailer.SendAsync(Mailable.AsInline<VerificationEmailViewModel>()
                .To(recipient)
                .Subject("CleanIAM - Email verification")
                .View("Api/Emails/VerificationEmailView.cshtml", new VerificationEmailViewModel
                {
                    Recipient = recipient,
                    VerificationUrl = verificationUrl
                }));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Failed to send email");
            return Result.Error("Failed to send email", HttpStatusCode.InternalServerError);
        }

        return Result.Ok();
    }

    public async Task<Result> SendPasswordResetEmailAsync(EmailRecipient recipient, string passwordResetUrl)
    {
        try
        {
            await mailer.SendAsync(Mailable.AsInline<PasswordResetEmailViewModel>()
                .To(recipient)
                .Subject("CleanIAM - Password reset")
                .View("Api/Emails/PasswordResetEmailView.cshtml", new PasswordResetEmailViewModel
                {
                    Recipient = recipient,
                    PasswordResetUrl = passwordResetUrl
                }));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Failed to send email");
            return Result.Error("Failed to send email", HttpStatusCode.InternalServerError);
        }

        return Result.Ok();
    }

    public async Task<Result> SendInvitationEmailAsync(EmailRecipient recipient, string invitationUrl)
    {
        try
        {
            await mailer.SendAsync(Mailable.AsInline<InvitationEmailViewModel>()
                .To(recipient)
                .Subject("CleanIAM - Invitation")
                .View("Api/Emails/InvitationEmailView.cshtml", new InvitationEmailViewModel
                {
                    Recipient = recipient,
                    InvitationUrl = invitationUrl
                }));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Failed to send email");
            return Result.Error("Failed to send email", HttpStatusCode.InternalServerError);
        }

        return Result.Ok();
    }
}