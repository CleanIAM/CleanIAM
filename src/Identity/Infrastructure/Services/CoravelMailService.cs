using System.Net;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Interfaces;
using Identity.Api.Mails;
using Identity.Application.Interfaces;
using Identity.Core.Mails;
using SharedKernel.Infrastructure;

namespace Identity.Infrastructure.Services;

/// <summary>
/// 
/// </summary>
/// <param name="mailer"></param>
public class CoravelMailService(IMailer mailer, ILogger<CoravelMailService> logger) : IMailService
{
    public async Task<Result> SendVerificationEmailAsync(MailReceiver receiver, Guid verificationRequestId)
    {    
        try
        {
            await mailer.SendAsync(Mailable.AsInline<VerificationEmailViewModel>()
                    .To(receiver)
                    .Subject("CleanIAM - Email verification")
                    .View("Api/Mails/VerificationEmailView.cshtml", new VerificationEmailViewModel()));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Failed to send email");
            return Result.Error("Failed to send email", HttpStatusCode.InternalServerError);
        }
        
        return Result.Ok();
    }
    
    public async Task<Result> SendPasswordResetEmailAsync(MailReceiver receiver, Guid passwordResetRequestId)
    {
        
        try
        {
            await mailer.SendAsync(Mailable.AsInline<PasswordResetEmailViewModel>()
                .To(receiver)
                .Subject("CleanIAM - Password reset")
                .View("Api/Mails/ResetPasswordEmailView.cshtml", new PasswordResetEmailViewModel()));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Failed to send email");
            return Result.Error("Failed to send email", HttpStatusCode.InternalServerError);
        }
        
        return Result.Ok();
    }
}