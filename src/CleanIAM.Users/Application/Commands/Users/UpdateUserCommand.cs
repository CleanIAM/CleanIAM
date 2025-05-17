using Mapster;
using Marten;
using CleanIAM.SharedKernel.Core;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Users.Core;
using CleanIAM.Users.Core.Events.Users;
using Wolverine;

namespace CleanIAM.Users.Application.Commands.Users;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, UserRole[]? Roles);

public class UpdateUserCommandHandler
{
    public static async Task<Result<User>> LoadAsync(UpdateUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found", StatusCodes.Status404NotFound);

        return Result.Ok(user);
    }

    public static async Task<Result<UserUpdated>> HandleAsync(UpdateUserCommand command, Result<User> loadResult,
        IDocumentSession session, IMessageBus bus, ILogger<UpdateUserCommandHandler> logger)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        var updatedUser = command.Adapt(user);
        session.Store(updatedUser);
        await session.SaveChangesAsync();

        // Log the user update action
        logger.LogInformation("User {Id} updated", updatedUser.Id);

        var userUpdated = updatedUser.Adapt<UserUpdated>();
        await bus.PublishAsync(userUpdated);
        return Result.Ok(userUpdated);
    }
}