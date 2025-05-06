using System.Net;
using Identity.Core.Events;
using Identity.Core.Users;
using Marten;
using SharedKernel.Infrastructure;

namespace Identity.Application.Commands.Invitations;

/// <summary>
/// Command to create user invitation.
/// </summary>
/// <param name="Id">Id of the user</param>
/// <param name="Email">Email of the user</param>
/// <param name="FirstName">First name of the user</param>
/// <param name="LastName">Last name of the user</param>
public record CreateUserInvitationCommand(Guid Id, string Email, string FirstName, string LastName);

public class CreateUserInvitationCommandHandler
{
    public static async Task<Result<IdentityUser>> LoadAsync(CreateUserInvitationCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var user = await session.LoadAsync<IdentityUser>(command.Email, cancellationToken);
        if (user is not null)
            return Result.Error("User with given email already exists", HttpStatusCode.BadRequest);

        return Result.Ok(user);
    }

    public static async Task<Result<UserInvitationCreated>> HandleAsync(CreateUserInvitationCommand command,
        Result<IdentityUser> loadResult,
        IDocumentSession documentSession, CancellationToken cancellationToken)
    {
        return Result.From(loadResult);
        // if (loadResult.IsError())
        //
        // var newUser = command.Adapt<User>();
        // newUser.Id = Guid.NewGuid();
        // newUser.Email = command.Email.ToLowerInvariant(); // Normalize email
        // newUser.Roles = [UserRole.User];
        //
        // documentSession.Store(newUser);
        // await documentSession.SaveChangesAsync(cancellationToken);
        //
        // return Result.Ok(newUser);
    }
}