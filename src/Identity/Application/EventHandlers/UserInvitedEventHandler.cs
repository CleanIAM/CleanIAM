using Events.Core.Events;
using Identity.Application.Commands.Invitations;
using Identity.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Infrastructure;
using Wolverine;
using UserInvitationCreated = Identity.Core.Events.UserInvitationCreated;

namespace Identity.Application.EventHandlers;

public class UserInvitedEventHandler
{
    public static async Task Handle(UserInvited userInvitedEvent, IDocumentSession session, IMessageBus bus,
        CancellationToken cancellationToken)
    {
        // Create a new user account
        var user = userInvitedEvent.Adapt<IdentityUser>();
        user.IsInvitePending = true;
        session.Store(user);
        await session.SaveChangesAsync(cancellationToken);

        // Create invite
        var command = user.Adapt<CreateUserInvitationCommand>();
        var res = await bus.InvokeAsync<Result<UserInvitationCreated>>(command, cancellationToken);
        //TODO: handle error
    }
}