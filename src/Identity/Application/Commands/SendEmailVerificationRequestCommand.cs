using System.Net;
using Identity.Application.Interfaces;
using Identity.Core;
using Identity.Core.Events;
using Identity.Core.Mails;
using Identity.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Application.Interfaces;
using SharedKernel.Infrastructure;
using UrlShortner.Application.Commands;
using UrlShortner.Core.Events;
using Wolverine;

namespace Identity.Application.Commands;

public record SendEmailVerificationRequestCommand(Guid UserId);

/// <summary>
/// This handler loads or creates a new email verification request and sends email verification.
/// </summary>
public class SendEmailVerificationRequestCommandHandler
{
    public static async Task<Result<EmailVerificationReqest>> LoadAsync(SendEmailVerificationRequestCommand command,
        IQuerySession querySession)
    {
        // Check if the user for a given request exists
        var user = await querySession.LoadAsync<User>(command.UserId);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);


        var request = querySession.Query<EmailVerificationReqest>()
            .FirstOrDefault(x => x.UserId == user.Id);

        // If request for given user doesn't exist, create a new one
        if (request is null)
        {
            var newRequest = user.Adapt<EmailVerificationReqest>();
            newRequest.Id = Guid.NewGuid();
            newRequest.LastEmailsSendAt = DateTime.MinValue;
            newRequest.UserId = user.Id;
            return Result.Ok(newRequest);
        }

        // If the request exists, check if it wasn't sent too recently
        var timeSinceLastEmail = DateTime.UtcNow - request.LastEmailsSendAt;
        if (timeSinceLastEmail < IdentityConstants.VerificationEmailDelay)
            return Result.Error(
                $"Email verification request already send, " +
                $"you need to wait {(IdentityConstants.VerificationEmailDelay - timeSinceLastEmail).Minutes} minutes " +
                $"before you request new email.",
                HttpStatusCode.BadRequest);

        return Result.Ok(request);
    }

    public static async Task<Result<EmailVerificationRequestSent>> HandleAsync(
        SendEmailVerificationRequestCommand command,
        Result<EmailVerificationReqest> result, IEmailService emailService, IAppConfiguration configuration,
        IDocumentSession documentSession, IMessageBus bus)
    {
        if (result.IsError())
            return Result.From(result);
        var request = result.Value;

        // Build the email verification url
        var verificationUrl = $"{configuration.IdentityBaseUrl}/email-verification/{request.Id.ToString()}";

        // Shorten the url if shortening is enabled
        if (configuration.UseUrlShortener)
        {
            var shortenUrlCommand = new ShortenUrlCommand(verificationUrl);
            var shortingRes = await bus.InvokeAsync<Result<UrlShortened>>(shortenUrlCommand);
            if (shortingRes.IsError())
                return Result.From(shortingRes);
            verificationUrl = shortingRes.Value.ShortenedUrl;
        }

        // Send verification email
        var res = await emailService.SendVerificationEmailAsync(request.Adapt<EmailRecipient>(), verificationUrl);
        if (res.IsError())
            return res;

        // Upsert request
        request.LastEmailsSendAt = DateTime.UtcNow;
        documentSession.Store(request);
        await documentSession.SaveChangesAsync();

        // Publish event
        var verificationEmailSent = request.Adapt<EmailVerificationRequestSent>();
        await bus.PublishAsync(verificationEmailSent);
        return Result.Ok(verificationEmailSent);
    }
}