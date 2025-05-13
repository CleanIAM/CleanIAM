using CleanIAM.Events.Core.Events.Identity;
using CleanIAM.Users.Core;
using Marten;

namespace CleanIAM.Users.Application.EventHandlers;

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
        user.EmailVerified = true;
        session.Store(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}