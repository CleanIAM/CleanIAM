using System.Net;
using Identity.Application.Interfaces;
using Identity.Core.Events;
using Identity.Core.Requests;
using Identity.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Infrastructure;
using Wolverine;

namespace Identity.Application.Commands.Invitations;

/// <summary>
/// Command to set up a user account.
/// </summary>
/// <param name="RequirestId">Id of the invitation request</param>
/// <param name="Passowrd">New users password</param>
public record SetupUserAccountCommand(Guid RequirestId, string Passowrd);

public class SetupUserAccountCommandHandler
{
    public static async Task<Result<(InvitationRequest, IdentityUser)>> LoadAsync(SetupUserAccountCommand command,
        IQuerySession session, CancellationToken cancellationToken)
    {
        var request = await session.LoadAsync<InvitationRequest>(command.RequirestId, cancellationToken);
        if (request is null)
            return Result.Error("Invitation request not found", HttpStatusCode.BadRequest);

        var user = await session.LoadAsync<IdentityUser>(request.UserId, cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.BadRequest);

        return Result.Ok((request, user));
    }

    public static async Task<Result<UserAccountSetup>> HandleAsync(SetupUserAccountCommand command,
        Result<(InvitationRequest, IdentityUser)> loadResult, IPasswordHasher passwordHasher, IMessageBus bus,
        IDocumentSession session, CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var (request, user) = loadResult.Value;

        // Set the password
        user.HashedPassword = passwordHasher.Hash(command.Passowrd);

        // Set the user as setup
        user.SetUserAsSetup();
        user.SetUserEmailAsVerified();

        // update the use in database
        session.Store(user);
        session.Delete(request);
        await session.SaveChangesAsync(cancellationToken);

        // Publish the user setup event
        var userSetup = user.Adapt<UserAccountSetup>();
        await bus.PublishAsync(userSetup);
        return Result.Ok(userSetup);
    }
}