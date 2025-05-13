using CleanIAM.Events.Core.Events.Users;
using CleanIAM.Identity.Application.Commands.Invitations;
using CleanIAM.Identity.Core.Users;
using Mapster;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using Wolverine;
using Events_UserInvitationCreated = CleanIAM.Identity.Core.Events.UserInvitationCreated;
using UserInvitationCreated = CleanIAM.Identity.Core.Events.UserInvitationCreated;

namespace CleanIAM.Identity.Application.EventHandlers.Users;

public class UserInvitedEventHandler
{
    public static async Task Handle(UserInvited userInvitedEvent, IDocumentSession session, IMessageBus bus,
        CancellationToken cancellationToken, ILogger logger)
    {
        logger.LogDebug("Handling UserInvited event for user [{}]", userInvitedEvent.Email);
        // Create a new user account if it doesn't exist
        var user = userInvitedEvent.Adapt<IdentityUser>();
        user.TenantId = Guid.TryParse(session.TenantId, out var tenantId) ? tenantId : Guid.Empty;
        user.IsInvitePending = true;
        session.Store(user);
        await session.SaveChangesAsync(cancellationToken);


        // Create invite
        var command = user.Adapt<CreateUserInvitationCommand>();
        var res = await bus.InvokeAsync<Result<Events_UserInvitationCreated>>(command, cancellationToken);
        if (res.IsError())
            logger.LogError("Failed to create user invitation: {Error}({})", res.ErrorValue.Message,
                res.ErrorValue.Code);
        else
            logger.LogInformation("User invitation created successfully for user [{Email}]", user.Email);
        //TODO: handle error
    }
}