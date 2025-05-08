using Mapster;
using Marten;
using SharedKernel.Infrastructure.Utils;
using Users.Core;
using Users.Core.Events.Users;
using Wolverine;

namespace Users.Application.Commands.Users;

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
            return Result.Error("User not found");

        return Result.Ok(user);
    }

    public static async Task<Result<UserUpdated>> Handle(UpdateUserSimpleCommand command, Result<User> loadResult,
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