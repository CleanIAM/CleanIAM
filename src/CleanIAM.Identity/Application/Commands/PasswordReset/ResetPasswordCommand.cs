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
    public static async Task<Result<PasswordResetRequest>> LoadAsync(ResetPasswordCommand command,
        IQuerySession querySession, CancellationToken cancellationToken)
    {
        var request = await querySession.LoadAsync<PasswordResetRequest>(command.RequestId, cancellationToken);

        if (request is null)
            return Result.Error("Request not found", HttpStatusCode.NotFound);

        return Result.Ok(request);
    }

    public static async Task<Result<Core.Events.PasswordReset>> HandleAsync(ResetPasswordCommand command,
        Result<PasswordResetRequest> result, IDocumentSession documentSession, IMessageBus bus,
        IPasswordHasher passwordHasher, CancellationToken cancellationToken,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        if (result.IsError())
            return Result.From(result);
        var request = result.Value;

        // Get user from the database
        var user = await documentSession.LoadAsync<IdentityUser>(request.UserId, cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        // Update user password
        user.HashedPassword = passwordHasher.Hash(command.NewPassword);
        documentSession.Delete(request);
        documentSession.Update(user);
        await documentSession.SaveChangesAsync(cancellationToken);

        // Log the password reset
        logger.LogInformation("User {Id} password reset", user.Id);

        // Publish event
        var passwordReset = user.Adapt<Core.Events.PasswordReset>();
        await bus.PublishAsync(passwordReset);
        return Result.Ok(passwordReset);
    }
}