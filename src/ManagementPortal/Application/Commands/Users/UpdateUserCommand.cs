using ManagementPortal.Core;
using ManagementPortal.Core.Events.Users;
using Mapster;
using Marten;
using SharedKernel.Core;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.Users;

public record UpdateUserCommand(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);

public class UpdateUserCommandHandler
{
    public static async Task<Result<User>> LoadAsync(UpdateUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found");

        return Result.Ok();
    }

    public static async Task<Result<UserUpdated>> Handle(UpdateUserCommand command, Result<User> res,
        IDocumentSession session, IMessageBus bus)
    {
        if (res.IsError())
            return Result.From(res);

        var updatedUser = command.Adapt<User>();
        session.Store(updatedUser);
        await session.SaveChangesAsync();

        var userUpdated = updatedUser.Adapt<UserUpdated>();
        await bus.PublishAsync(userUpdated);
        return Result.Ok(userUpdated);
    }
}