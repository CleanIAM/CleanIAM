using Mapster;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Users.Core;
using CleanIAM.Users.Core.Events.Users;
using Wolverine;

namespace CleanIAM.Users.Application.Commands.Users;

/// <summary>
/// Command to update a user's first and last name.
/// </summary>
/// <param name="Id">Id of the user to update</param>
/// <param name="FirstName">New First name</param>
/// <param name="LastName">New last name</param>
public record UpdateUserSimpleCommand(Guid Id, string FirstName, string LastName);

public class UpdateUserSimpleCommandHandler
{
    public static async Task<Result<User>> LoadAsync(UpdateUserSimpleCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found", StatusCodes.Status404NotFound);

        return Result.Ok(user);
    }

    public static async Task<Result<UserUpdated>> HandleAsync(UpdateUserSimpleCommand command, Result<User> loadResult,
        IDocumentSession session, IMessageBus bus, ILogger<UpdateUserSimpleCommandHandler> logger)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        var updatedUser = command.Adapt(user);
        session.Store(updatedUser);
        await session.SaveChangesAsync();

        // Log the update
        logger.LogInformation("User {Id} updated successfully", updatedUser.Id);

        var userUpdated = updatedUser.Adapt<UserUpdated>();
        await bus.PublishAsync(userUpdated);
        return Result.Ok(userUpdated);
    }
}