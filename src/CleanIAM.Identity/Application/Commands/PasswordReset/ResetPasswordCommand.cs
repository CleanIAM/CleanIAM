using System.Net;
using CleanIAM.Identity.Application.Interfaces;
using CleanIAM.Identity.Core.Requests;
using CleanIAM.Identity.Core.Users;
using Mapster;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace CleanIAM.Identity.Application.Commands.PasswordReset;

/// <summary>
/// Reset users password
/// </summary>
/// <param name="RequestId">Id of the password reset request</param>
/// <param name="NewPassword">The new password</param>
public record ResetPasswordCommand(Guid RequestId, string NewPassword);

/// <summary>
/// This handler loads the password reset request and resets the user password.
/// </summary>
public class ResetPasswordCommandHandler
{
    public static async Task<Result<(PasswordResetRequest, IdentityUser)>> LoadAsync(ResetPasswordCommand command,
        IQuerySession querySession, CancellationToken cancellationToken)
    {
        var request = await querySession.LoadAsync<PasswordResetRequest>(command.RequestId, cancellationToken);

        if (request is null)
            return Result.Error("Request not found", HttpStatusCode.NotFound);
        
        var user = await querySession.Query<IdentityUser>()
            .FirstOrDefaultAsync(user => user.Id == request.UserId && user.AnyTenant(), cancellationToken);

        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);
        
        return Result.Ok((request, user));
    }

    public static async Task<Result<Core.Events.PasswordReset>> HandleAsync(ResetPasswordCommand command,
        Result<(PasswordResetRequest, IdentityUser)> loadResult, IDocumentStore store, IMessageBus bus,
        IPasswordHasher passwordHasher, CancellationToken cancellationToken,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var (request, user) = loadResult.Value;


        // Update user password
        user.HashedPassword = passwordHasher.Hash(command.NewPassword);
        var session = store.LightweightSession(user.TenantId.ToString());
        session.Delete(request);
        session.Update(user);
        await session.SaveChangesAsync(cancellationToken);

        // Log the password reset
        logger.LogInformation("User {Id} password reset", user.Id);

        // Publish event
        var passwordReset = user.Adapt<Core.Events.PasswordReset>();
        await bus.PublishAsync(passwordReset, new DeliveryOptions{TenantId = user.TenantId.ToString()});
        return Result.Ok(passwordReset);
    }
}