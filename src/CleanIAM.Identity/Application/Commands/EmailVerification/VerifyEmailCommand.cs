using System.Net;
using CleanIAM.Identity.Core.Events;
using CleanIAM.Identity.Core.Requests;
using CleanIAM.Identity.Core.Users;
using Mapster;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace CleanIAM.Identity.Application.Commands.EmailVerification;

/// <summary>
/// Sets the email-verified status for the user.
/// </summary>
/// <param name="RequestId">Id of the email verification request</param>
public record VerifyEmailCommand(Guid RequestId);

/// <summary>
/// This handler loads the email verification request and sets the email-verified status for the user.
/// </summary>
public class VerifyEmailCommandHandler
{
    public static async Task<Result<(EmailVerificationRequest, IdentityUser)>> LoadAsync(VerifyEmailCommand command,
        IQuerySession querySession, CancellationToken cancellationToken)
    {
        var request = await querySession.LoadAsync<EmailVerificationRequest>(command.RequestId, cancellationToken);
        if (request is null)
            return Result.Error("Request not found", HttpStatusCode.NotFound);

        var user = await querySession.LoadAsync<IdentityUser>(request.UserId, cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok((request, user));
    }

    public static async Task<Result<UserEmailVerified>> HandleAsync(VerifyEmailCommand command,
        Result<(EmailVerificationRequest, IdentityUser)> result, IDocumentSession documentSession, IMessageBus bus,
        CancellationToken cancellationToken, ILogger<VerifyEmailCommandHandler> logger)
    {
        if (result.IsError())
            return Result.From(result);
        var (request, user) = result.Value;


        // Update email verification status
        user.EmailVerified = true;
        documentSession.Delete(request);
        documentSession.Update(user);
        await documentSession.SaveChangesAsync(cancellationToken);

        // Log the email verification
        logger.LogInformation("User {Id} email verified", user.Id);

        // Publish event
        var emailVerified = user.Adapt<UserEmailVerified>();
        await bus.PublishAsync(emailVerified);
        return Result.Ok(emailVerified);
    }
}