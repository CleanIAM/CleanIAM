using Mapster;
using Marten;
using SharedKernel.Core;
using SharedKernel.Infrastructure.Utils;
using Users.Core;
using Users.Core.Events.Users;
using Wolverine;

namespace Users.Application.Commands.Users;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, UserRole[]? Roles);

public class UpdateUserCommandHandler
{
    public static async Task<Result<User>> LoadAsync(UpdateUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found");

        return Result.Ok(user);
    }

    public static async Task<Result<UserUpdated>> HandleAsync(UpdateUserCommand command, Result<User> loadResult,
        IDocumentSession session, IMessageBus bus)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        var updatedUser = command.Adapt(user);
        session.Store(updatedUser);
        await session.SaveChangesAsync();

        var userUpdated = updatedUser.Adapt<UserUpdated>();
        await bus.PublishAsync(userUpdated);
        return Result.Ok(userUpdated);
    }
}