using ManagementPortal.Core.Events.Users;
using ManagementPortal.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Core.Users;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.Users;

public record DeleteUserCommand(Guid Id);

public class DeleteUserCommandHandler
{
    public async Task<Result> LoadAsync(DeleteUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found");

        return Result.Ok();
    }

    public async Task<Result<UserDeleted>> Handle(DeleteUserCommand command, Result loadRes, IDocumentSession session,
        IMessageBus bus, CancellationToken cancellationToken)
    {
        if (loadRes.IsError())
            return loadRes;

        session.Delete<User>(command.Id);
        await session.SaveChangesAsync(cancellationToken);

        var userDeleted = command.Adapt<UserDeleted>();
        await bus.PublishAsync(userDeleted);
        return Result.Ok(userDeleted);
    }
}