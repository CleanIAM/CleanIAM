using System.Net;
using ManagementPortal.Core;
using ManagementPortal.Core.Events.Users;
using Mapster;
using Marten;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.Users;

/// <summary>
/// Represents a command to enable a user by their unique identifier.
/// </summary>
/// <param name="Id">Id of the user to enable.</param>
public record EnableUserCommand(Guid Id);

public class EnableUserCommandHandler
{
    public static async Task<Result<User>> LoadAsync(EnableUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok(user);
    }

    public static async Task<Result<UserEnabled>> HandleAsync(EnableUserCommand command, Result<User> loadResult,
        IMessageBus bus, IDocumentSession session)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        // Enable user
        user.IsDisabled = false;
        session.Update(user);
        await session.SaveChangesAsync();

        // Publish user enabled event
        var userEnabledEvent = user.Adapt<UserEnabled>();
        await bus.PublishAsync(userEnabledEvent);
        return Result.Ok(userEnabledEvent);
    }
}