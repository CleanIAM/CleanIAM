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

        var user = await querySession.Query<IdentityUser>().FirstOrDefaultAsync(user => user.Id == request.UserId && user.AnyTenant(), cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok((request, user));
    }

    public static async Task<Result<UserEmailVerified>> HandleAsync(VerifyEmailCommand command,
        Result<(EmailVerificationRequest, IdentityUser)> result, IDocumentStore store, IMessageBus bus,
        CancellationToken cancellationToken, ILogger<VerifyEmailCommandHandler> logger)
    {
        if (result.IsError())
            return Result.From(result);
        var (request, user) = result.Value;

        

        // Update email verification status
        user.EmailVerified = true;
        
        var session = store.LightweightSession(user.TenantId.ToString());
        session.Delete(request);
        session.Update(user);
        await session.SaveChangesAsync(cancellationToken);

        // Log the email verification
        logger.LogInformation("User {Id} email verified", user.Id);

        // Publish event
        var emailVerified = user.Adapt<UserEmailVerified>();
        await bus.PublishAsync(emailVerified, new DeliveryOptions{TenantId = user.TenantId.ToString()});
        return Result.Ok(emailVerified);
    }
}