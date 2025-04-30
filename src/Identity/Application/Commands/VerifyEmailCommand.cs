using System.Net;
using Identity.Core;
using Identity.Core.Events;
using Identity.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Infrastructure;
using Wolverine;

namespace Identity.Application.Commands;

public record VerifyEmailCommand(Guid RequestId);

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

        var user = await documentSession.LoadAsync<User>(request.UserId, cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        user.EmailVerified = true;

        documentSession.Delete(request);
        documentSession.Update(user);
        await documentSession.SaveChangesAsync(cancellationToken);

        var emailVerified = user.Adapt<UserEmailVerified>();
        await bus.PublishAsync(emailVerified);
        return Result.Ok(emailVerified);
    }
}