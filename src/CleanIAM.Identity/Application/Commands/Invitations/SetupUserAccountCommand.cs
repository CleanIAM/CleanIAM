using System.Net;
using CleanIAM.Identity.Application.Interfaces;
using CleanIAM.Identity.Core.Events;
using CleanIAM.Identity.Core.Requests;
using CleanIAM.Identity.Core.Users;
using Mapster;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace CleanIAM.Identity.Application.Commands.Invitations;

/// <summary>
/// Command to set up a user account.
/// </summary>
/// <param name="RequirestId">Id of the invitation request</param>
/// <param name="Password">New users password</param>
public record SetupUserAccountCommand(Guid RequirestId, string Password);

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
        IDocumentSession session, CancellationToken cancellationToken, ILogger<SetupUserAccountCommandHandler> logger)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var (request, user) = loadResult.Value;

        // Set the password
        user.HashedPassword = passwordHasher.Hash(command.Password);

        // Set the user as setup
        user.SetUserAsSetup();
        user.SetUserEmailAsVerified();

        // update the use in database
        session.Store(user);
        session.Delete(request);
        await session.SaveChangesAsync(cancellationToken);

        // Log the user setup
        logger.LogInformation("User {Id} setup", user.Id);

        // Publish the user setup event
        var userSetup = user.Adapt<UserAccountSetup>();
        await bus.PublishAsync(userSetup);
        return Result.Ok(userSetup);
    }
}