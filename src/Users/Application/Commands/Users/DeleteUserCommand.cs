using Mapster;
using Marten;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure.Utils;
using Users.Core;
using Users.Core.Events.Users;
using Wolverine;

namespace Users.Application.Commands.Users;

/// <summary>
/// Command to delete a user.
/// This event implement user revocation and deletion.
/// All tokens associated with the user will be revoked.
/// </summary>
/// <param name="Id">Id of the user to delete</param>
public record DeleteUserCommand(Guid Id);

public class DeleteUserCommandHandler
{
    public static async Task<Result> LoadAsync(DeleteUserCommand command, IQuerySession session)
    {
        var user = await session.LoadAsync<User>(command.Id);
        if (user == null)
            return Result.Error("User not found");

        return Result.Ok();
    }

    public static async Task<Result<UserDeleted>> HandleAsync(DeleteUserCommand command, Result loadResult,
        IDocumentSession session,
        IMessageBus bus, CancellationToken cancellationToken, IOpenIddictTokenManager tokenManager)
    {
        if (loadResult.IsError())
            return loadResult;

        // Delete the user from the database
        session.Delete<User>(command.Id);
        await session.SaveChangesAsync(cancellationToken);

        var userDeleted = command.Adapt<UserDeleted>();
        await bus.PublishAsync(userDeleted);
        return Result.Ok(userDeleted);
    }
}