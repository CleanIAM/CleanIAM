using ManagementPortal.Core.Events.Users;
using ManagementPortal.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Core;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.Users;

/// <summary>
/// Command to invite a user.
/// </summary>
/// <param name="Id">If of invited user</param>
/// <param name="Email">Email of invited user</param>
/// <param name="FirstName">First name of invited user</param>
/// <param name="LastName">Last name of invited user</param>
/// <param name="Roles">Roles of invited user</param>
public record InviteUserCommand(Guid Id, string Email, string FirstName, string LastName, UserRole[] Roles);

public class InviteUserCommandHandler
{
    public async Task<Result> LoadAsync(InviteUserCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var user = await session.Query<User>()
            .FirstOrDefaultAsync(u => u.Email == command.Email.ToLowerInvariant(), cancellationToken);
        if (user is not null)
            return Result.Error("User already exists", StatusCodes.Status400BadRequest);

        return Result.Ok();
    }

    public async Task<Result<UserInvited>> HandleAsync(InviteUserCommand command, Result laodResult, IMessageBus bus,
        IDocumentSession session, CancellationToken cancellationToken, ILogger logger)
    {
        if (laodResult.IsError())
            return laodResult;

        logger.LogDebug("Inviting user [{}]", command.Email);

        var user = command.Adapt<User>();
        user.IsInvitePending = true;
        user.Email = user.Email.ToLowerInvariant(); // Normalize email
        session.Store(user);
        await session.SaveChangesAsync(cancellationToken);

        var userInvited = user.Adapt<UserInvited>();
        await bus.PublishAsync(userInvited);
        return Result.Ok(userInvited);
    }
}