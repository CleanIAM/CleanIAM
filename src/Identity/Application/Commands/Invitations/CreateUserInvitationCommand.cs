using System.Net;
using Identity.Application.Interfaces;
using Identity.Core.Events;
using Identity.Core.Mails;
using Identity.Core.Requests;
using Identity.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Application.Interfaces;
using SharedKernel.Infrastructure.Utils;
using UrlShortner.Application.Commands;
using UrlShortner.Core.Events;
using Wolverine;

namespace Identity.Application.Commands.Invitations;

/// <summary>
/// Command to create user invitation.
/// </summary>
/// <param name="Id">Id of the user</param>
/// <param name="Email">Email of the user</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
public record CreateUserInvitationCommand(Guid Id, string Email, string FirstName, string LastName);

public class CreateUserInvitationCommandHandler
{
    public static async Task<Result<InvitationRequest>> LoadAsync(CreateUserInvitationCommand command,
        IQuerySession session, ILogger logger,
        CancellationToken cancellationToken)
    {
        var user = await session.LoadAsync<IdentityUser>(command.Id, cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.BadRequest);

        if (!user.IsInvitePending)
            return Result.Error("User already accepted the invite", StatusCodes.Status400BadRequest);

        var request = session.Query<InvitationRequest>()
            .FirstOrDefault(x => x.UserId == user.Id);

        // If request for given user doesn't exist, create a new one
        if (request is null)
        {
            logger.LogDebug("Creating new invitation request for user [{Email}]", command.Email);
            var newRequest = user.Adapt<InvitationRequest>();
            newRequest.Id = Guid.NewGuid();
            newRequest.LastEmailsSendAt = DateTime.MinValue;
            newRequest.UserId = user.Id;
            return Result.Ok(newRequest);
        }

        // If the request exists, check if it wasn't sent too recently
        var timeSinceLastEmail = DateTime.UtcNow - request.LastEmailsSendAt;
        if (timeSinceLastEmail < IdentityConstants.VerificationEmailDelay)
        {
            logger.LogDebug("Email already send, you need to wait {Minutes} minutesbefore you request new email.",
                (IdentityConstants.VerificationEmailDelay - timeSinceLastEmail).Minutes);
            return Result.Error(
                $"Email already send, " +
                $"you need to wait {(IdentityConstants.VerificationEmailDelay - timeSinceLastEmail).Minutes} minutes " +
                $"before you request new email.",
                HttpStatusCode.BadRequest);
        }

        logger.LogDebug("Using existing invitation request for user [{Email}]", command.Email);
        return Result.Ok(request);
    }

    public static async Task<Result<UserInvitationCreated>> HandleAsync(CreateUserInvitationCommand command,
        Result<InvitationRequest> loadResult, IDocumentSession session, IMessageBus bus, IEmailService emailService,
        IAppConfiguration configuration, CancellationToken cancellationToken, ILogger logger)
    {
        logger.LogDebug("Creating user invitation for {Email}", command.Email);

        if (loadResult.IsError())
            return Result.From(loadResult);
        var request = loadResult.Value;

        // Build the email verification url
        var invitationUrl = $"{configuration.IdentityBaseUrl}/invitations/{request.Id.ToString()}";

        // Shorten the url if shortening is enabled
        if (configuration.UseUrlShortener)
        {
            logger.LogDebug("Shortening url [{Url}]", invitationUrl);
            var shortenUrlCommand = new ShortenUrlCommand(invitationUrl);
            var shortingRes = await bus.InvokeAsync<Result<UrlShortened>>(shortenUrlCommand, cancellationToken);
            if (shortingRes.IsError())
                return Result.From(shortingRes);
            invitationUrl = shortingRes.Value.ShortenedUrl;
        }

        // Send invitation email
        var res = await emailService.SendInvitationEmailAsync(request.Adapt<EmailRecipient>(), invitationUrl);
        if (res.IsError())
            return res;

        // Upsert request
        request.LastEmailsSendAt = DateTime.UtcNow;
        session.Store(request);
        await session.SaveChangesAsync(cancellationToken);

        // Publish event
        var invitationCreated = request.Adapt<UserInvitationCreated>();
        await bus.PublishAsync(invitationCreated);
        return Result.Ok(invitationCreated);
    }
}