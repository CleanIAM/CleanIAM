using Events.Core.Events.Identity;
using Marten;
using Users.Core;

namespace Users.Application.EventHandlers;

public class UserAccountSetupEventHandler
{
    public static async Task Handle(UserAccountSetup accountSetup, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.LoadAsync<User>(accountSetup.Id, cancellationToken);
        if (user is null)
        {
            logger.LogError("[UserAccountSetupEventHandler]User with id {Id} doesn't exist", accountSetup.Id);
            return;
        }
        
        user.IsInvitePending = false;
        session.Store(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}