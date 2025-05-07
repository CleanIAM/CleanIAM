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
public record DisableMfaForUserCommand(Guid Id);

public class DisableMfaForUserCommandHandler
{
    public static async Task<Result<User>> LoadAsync(DisableMfaForUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found");

        return Result.Ok(user);
    }

    public static async Task<Result<MfaDisabledForUser>> Handle(DisableMfaForUserCommand command,
        Result<User> loadResult,
        IDocumentSession session, IMessageBus bus)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        user.IsMFAEnabled = false;
        session.Store(user);
        await session.SaveChangesAsync();

        var userUpdated = user.Adapt<MfaDisabledForUser>();
        await bus.PublishAsync(userUpdated);
        return Result.Ok(userUpdated);
    }
}