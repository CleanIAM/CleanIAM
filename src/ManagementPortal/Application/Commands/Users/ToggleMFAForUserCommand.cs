using ManagementPortal.Core.Events.Users;
using Mapster;
using Marten;
using SharedKernel.Core.Users;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.Users;

/// <summary>
/// Command to enable or disable MFA for a user
/// </summary>
/// <param name="Id">User Id</param>
/// <param name="enabled">is MFA enabled </param>
public record ToggleMFAForUserCommand(Guid Id, bool enabled);

public class ToggleMFAForUserCommandHandler
{
    public static async Task<Result<User>> LoadAsync(ToggleMFAForUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found");

        return Result.Ok(user);
    }

    public static async Task<Result<UserUpdated>> Handle(ToggleMFAForUserCommand command, Result<User> res,
        IDocumentSession session, IMessageBus bus)
    {
        if (res.IsError())
            return Result.From(res);
        var user = res.Value;

        user.IsMFAEnabled = command.enabled;
        session.Store(user);
        await session.SaveChangesAsync();

        var userUpdated = user.Adapt<UserUpdated>();
        await bus.PublishAsync(userUpdated);
        return Result.Ok(userUpdated);
    }
}