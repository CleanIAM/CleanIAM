using Mapster;
using Marten;
using OpenIddict.Abstractions;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Users.Core;
using CleanIAM.Users.Core.Events.Users;
using Wolverine;

namespace CleanIAM.Users.Application.Commands.Users;

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
            return Result.Error("User not found", StatusCodes.Status404NotFound);

        return Result.Ok();
    }

    public static async Task<Result<UserDeleted>> HandleAsync(DeleteUserCommand command, Result loadResult,
        IDocumentSession session, IMessageBus bus, CancellationToken cancellationToken,
        ILogger<DeleteUserCommandHandler> logger)
    {
        if (loadResult.IsError())
            return loadResult;

        // Delete the user from the database
        session.Delete<User>(command.Id);
        await session.SaveChangesAsync(cancellationToken);

        // Log the user deletion
        logger.LogInformation("User {Id} deleted", command.Id);

        var userDeleted = command.Adapt<UserDeleted>();
        await bus.PublishAsync(userDeleted);
        return Result.Ok(userDeleted);
    }
}