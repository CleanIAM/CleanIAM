using Identity.Core.Mails;
using SharedKernel.Infrastructure;

namespace Identity.Application.Interfaces;

public interface IMailService
{
    Task<Result> SendVerificationEmailAsync(MailReceiver receiver, string token);
}