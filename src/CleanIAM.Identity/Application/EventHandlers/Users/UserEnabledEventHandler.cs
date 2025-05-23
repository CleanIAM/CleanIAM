using CleanIAM.Events.Core.Events.Users;
using CleanIAM.Identity.Core.Users;
using Marten;

namespace CleanIAM.Identity.Application.EventHandlers.Users;

/// <summary>
/// Handles the UserEnabled event and enables the corresponding user from the database.
/// </summary>
public class UserEnabledEventHandler
{
    public static async Task Handle(UserEnabled userEnabled, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.LoadAsync<IdentityUser>(userEnabled.Id, cancellationToken);
        if (user is null)
        {
            logger.LogError("[UserEnabledEventHandler] User with id [{Id}] not found", userEnabled.Id);
            return;
        }

        user.Enable();
        session.Store(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}