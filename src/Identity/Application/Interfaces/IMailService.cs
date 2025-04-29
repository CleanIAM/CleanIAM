using Identity.Core.Mails;
using SharedKernel.Infrastructure;

namespace Identity.Application.Interfaces;

public interface IMailService
{
    Task<Result> SendVerificationEmailAsync(MailReceiver receiver, Guid verificationRequestId);
    Task<Result> SendPasswordResetEmailAsync(MailReceiver receiver, Guid passwordResetRequestId);
}