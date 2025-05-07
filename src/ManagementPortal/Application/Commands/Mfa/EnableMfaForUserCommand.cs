using System.Net;
using ManagementPortal.Core.Events.Mfa;
using ManagementPortal.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace ManagementPortal.Application.Commands.Mfa;

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
            return Result.Error("User not found");

        // Allow to enable mfa only if it is configured  
        if (!user.MfaConfig.IsMfaConfigured)
            return Result.Error("MFA is not configured", HttpStatusCode.BadRequest);

        return Result.Ok(user);
    }

    public static async Task<Result<MfaEnabledForUser>> Handle(EnableMfaForUserCommand command, Result<User> loadResult,
        IDocumentSession session, IMessageBus bus)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        user.IsMFAEnabled = true;
        session.Store(user);
        await session.SaveChangesAsync();

        var userUpdated = user.Adapt<MfaEnabledForUser>();
        await bus.PublishAsync(userUpdated);
        return Result.Ok(userUpdated);
    }
}