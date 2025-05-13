using CleanIAM.Events.Core.Events.Identity;
using CleanIAM.Users.Core;
using Marten;

namespace CleanIAM.Users.Application.EventHandlers;

public class UserEmailVerifiedEventHandler
{
    public static async Task Handle(UserEmailVerified emailVerified, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.LoadAsync<User>(emailVerified.Id, cancellationToken);
        if (user is null)
        {
            logger.LogError("[UserEmailVerifiedEventHandler]User with id {Id} doesn't exist", emailVerified.Id);
            return;
        }
        
        user.EmailVerified = true;
        session.Store(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}