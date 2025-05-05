using System.Net;
using Identity.Application.Interfaces;
using Identity.Core.Events;
using Identity.Core.Mails;
using Identity.Core.Requests;
using Mapster;
using Marten;
using SharedKernel.Application.Interfaces;
using SharedKernel.Core.Users;
using SharedKernel.Infrastructure;
using UrlShortner.Application.Commands;
using UrlShortner.Core.Events;
using Wolverine;

namespace Identity.Application.Commands.PasswordReset;

/// <summary>
/// Send password reset request.
/// </summary>
/// <param name="Email">Email of the user the request is requested for</param>
public record SendPasswordResetRequestCommand(string Email);

/// <summary>
/// This handler loads or creates a new password reset request and sends an email.
/// </summary>
public class SendPasswordResetRequestCommandHandler
{
    public static async Task<Result<PasswordResetRequest>> LoadAsync(SendPasswordResetRequestCommand command,
        IQuerySession querySession, CancellationToken cancellationToken)
    {
        // Normalize email
        var normalizedEmail = command.Email.ToLowerInvariant();
        // Check if the user for a given request exists
        var user = await querySession.Query<User>()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        var request = querySession.Query<PasswordResetRequest>()
            .FirstOrDefault(x => x.UserId == user.Id);

        // If request for given user doesn't exist, create a new one
        if (request is null)
        {
            var newRequest = user.Adapt<PasswordResetRequest>();
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

    public static async Task<Result<PasswordResetRequestSent>> HandleAsync(
        SendPasswordResetRequestCommand command,
        Result<PasswordResetRequest> result, IEmailService emailService, IAppConfiguration configuration,
        IDocumentSession documentSession, IMessageBus bus)
    {
        if (result.IsError())
            return Result.From(result);
        var request = result.Value;

        // Build the email verification url
        var verificationUrl = $"{configuration.IdentityBaseUrl}/password-reset/{request.Id.ToString()}";

        // Shorten the url if shortening is enabled
        if (configuration.UseUrlShortener)
        {
            var shortenUrlCommand = new ShortenUrlCommand(verificationUrl);
            var shortingRes = await bus.InvokeAsync<Result<UrlShortened>>(shortenUrlCommand);
            if (shortingRes.IsError())
                return Result.From(shortingRes);
            verificationUrl = shortingRes.Value.ShortenedUrl;
        }

        // Send password reset email
        var res = await emailService.SendPasswordResetEmailAsync(request.Adapt<EmailRecipient>(), verificationUrl);
        if (res.IsError())
            return res;

        // Upsert request
        request.LastEmailsSendAt = DateTime.UtcNow;
        documentSession.Store(request);
        await documentSession.SaveChangesAsync();

        // Publish event
        var passwordResetRequest = request.Adapt<PasswordResetRequestSent>();
        await bus.PublishAsync(passwordResetRequest);
        return Result.Ok(passwordResetRequest);
    }
}