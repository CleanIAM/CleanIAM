using System.Net;
using Mapster;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Users.Core;
using CleanIAM.Users.Core.Events.Mfa;
using Wolverine;

namespace CleanIAM.Users.Application.Commands.Mfa;

/// <summary>
/// Command to enable or disable MFA for a user
/// </summary>
/// <param name="Id">User Id</param>
public record EnableMfaForUserCommand(Guid Id);

public class EnableMfaForUserCommandHandler
{
    public static async Task<Result<User>> LoadAsync(EnableMfaForUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user is null)
            return Result.Error("User not found", StatusCodes.Status404NotFound);

        // Allow to enable mfa only if it is configured  
        if (!user.MfaConfig.IsMfaConfigured)
            return Result.Error("MFA is not configured", HttpStatusCode.BadRequest);

        return Result.Ok(user);
    }

    public static async Task<Result> HandleAsync(EnableMfaForUserCommand command,
        Result<User> loadResult, IDocumentSession session, IMessageBus bus,
        ILogger<EnableMfaForUserCommandHandler> logger)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        user.IsMFAEnabled = true;
        session.Store(user);
        await session.SaveChangesAsync();

        // Log the mfa configuration
        logger.LogInformation("User {Id} mfa configuration enabled", user.Id);

        var userUpdated = user.Adapt<MfaEnabledForUser>();
        await bus.PublishAsync(userUpdated);
        return Result.Ok();
    }
}