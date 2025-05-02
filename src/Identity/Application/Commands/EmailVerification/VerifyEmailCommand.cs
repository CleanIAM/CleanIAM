using System.Net;
using Identity.Core.Events;
using Identity.Core.Requests;
using Identity.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Infrastructure;
using Wolverine;

namespace Identity.Application.Commands.EmailVerification;

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
    public static async Task<Result<EmailVerificationReqest>> LoadAsync(VerifyEmailCommand command,
        IQuerySession querySession, CancellationToken cancellationToken)
    {
        var request = await querySession.LoadAsync<EmailVerificationReqest>(command.RequestId, cancellationToken);

        if (request is null)
            return Result.Error("Request not found", HttpStatusCode.NotFound);

        return Result.Ok(request);
    }

    public static async Task<Result<UserEmailVerified>> HandleAsync(VerifyEmailCommand command,
        Result<EmailVerificationReqest> result, IDocumentSession documentSession, IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (result.IsError())
            return Result.From(result);
        var request = result.Value;

        // Get user from the database
        var user = await documentSession.LoadAsync<User>(request.UserId, cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        // Update email verification status
        user.EmailVerified = true;
        documentSession.Delete(request);
        documentSession.Update(user);
        await documentSession.SaveChangesAsync(cancellationToken);

        // Publish event
        var emailVerified = user.Adapt<UserEmailVerified>();
        await bus.PublishAsync(emailVerified);
        return Result.Ok(emailVerified);
    }
}