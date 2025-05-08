using Mapster;
using Marten;
using SharedKernel.Infrastructure.Utils;
using Users.Core;
using Users.Core.Events.Users;
using Wolverine;

namespace Users.Application.Commands.Users;

/// <summary>
/// Command to trigger resend invitation email to a user
/// </summary>
/// <param name="Id"></param>
public record ResendInvitationEmailCommand(Guid Id);

public class ResendInvitationEmailCommandHandler
{
    public static async Task<Result<User>> LoadAsync(ResendInvitationEmailCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var user = await session.LoadAsync<User>(command.Id, cancellationToken);
        if (user is null)
            return Result.Error("User not found", StatusCodes.Status404NotFound);
        return Result.Ok(user);
    }

    public static async Task<Result<UserInvited>> HandleAsync(ResendInvitationEmailCommand command,
        Result<User> loadResult,
        IDocumentSession session, IMessageBus bus, CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        // Publishing userInvited event will trigger the email sending process
        var userInvited = user.Adapt<UserInvited>();
        await bus.PublishAsync(userInvited);

        return Result.Ok(userInvited);
    }
}