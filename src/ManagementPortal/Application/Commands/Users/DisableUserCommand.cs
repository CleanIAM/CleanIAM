using System.Net;
using ManagementPortal.Core.Events.Users;
using Mapster;
using Marten;
using SharedKernel.Core.Users;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.Users;

/// <summary>
/// Represents a command to disable a user within the system.
/// </summary>
/// <param name="Id">Id of the user to be disabled.</param>
public record DisableUserCommand(Guid Id);

public class DisableUserCommandHandler
{
    public static async Task<Result<User>> LoadAsync(DisableUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok(user);
    }

    public static async Task<Result<UserDisabled>> HandleAsync(DisableUserCommand command, Result<User> loadResult,
        IMessageBus bus, IDocumentSession session)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        // Disable user
        user.IsDisabled = true;
        session.Update(user);
        await session.SaveChangesAsync();

        // Publish user disabled event
        var userDisabledEvent = user.Adapt<UserDisabled>();
        await bus.PublishAsync(userDisabledEvent);
        return Result.Ok(userDisabledEvent);
    }
}