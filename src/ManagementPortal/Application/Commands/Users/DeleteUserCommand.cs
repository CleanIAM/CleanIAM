using ManagementPortal.Core.Events.Users;
using ManagementPortal.Core.Users;
using Mapster;
using Marten;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace ManagementPortal.Application.Commands.Users;

/// <summary>
/// Command to delete a user.
/// This event implement user revocation and deletion.
/// All tokens associated with the user will be revoked.
/// </summary>
/// <param name="Id">Id of the user to delete</param>
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

    public async Task<Result<UserDeleted>> Handle(DeleteUserCommand command, Result loadResult,
        IDocumentSession session,
        IMessageBus bus, CancellationToken cancellationToken, IOpenIddictTokenManager tokenManager)
    {
        if (loadResult.IsError())
            return loadResult;

        // Delete the user from the database
        session.Delete<User>(command.Id);
        await session.SaveChangesAsync(cancellationToken);

        // Revoke all tokens associated with the user
        await tokenManager.RevokeBySubjectAsync(command.Id.ToString(), cancellationToken);

        var userDeleted = command.Adapt<UserDeleted>();
        await bus.PublishAsync(userDeleted);
        return Result.Ok(userDeleted);
    }
}