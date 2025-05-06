using ManagementPortal.Core.Events.Users;
using Mapster;
using Marten;
using SharedKernel.Core;
using SharedKernel.Core.Users;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.Users;

public record CreateNewUserCommand(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);

public class CreateNewUserCommandHandler
{
    public static async Task<Result> LoadAsync(CreateNewUserCommand command, IQuerySession session)
    {
        var user = await session.Query<User>().Where(u => u.Email == command.Email).FirstOrDefaultAsync();
        if (user != null)
            return Result.Error("User with given email already exists", StatusCodes.Status400BadRequest);

        return Result.Ok();
    }

    public static async Task<Result<UserCreated>> Handle(CreateNewUserCommand command, Result res,
        IDocumentSession session, IMessageBus bus)
    {
        if (res.IsError())
            return res;

        var user = command.Adapt<User>();
        session.Store(user);
        await session.SaveChangesAsync();

        var userCreated = user.Adapt<UserCreated>();
        await bus.PublishAsync(userCreated);
        return Result.Ok();
    }
}