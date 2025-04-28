using System.Net;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Interfaces;
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
    public async Task<Result> SendVerificationEmailAsync(MailReceiver receiver, string token)
    {
        try
        {
            await mailer.SendAsync(Mailable.AsInline<MailReceiver>()
                    .To(receiver)
                    .Subject("Email verification")
                    .View("Api/Mails/VerificationEmail.cshtml"));
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Error, ex, "Failed to send email");
            return Result.Error("Failed to send email", HttpStatusCode.InternalServerError);
        }
        
        return Result.Ok();
    }
}