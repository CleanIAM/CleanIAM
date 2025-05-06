using Events.Core.Events.ManagementPortal.Users;
using Identity.Core.Users;
using Marten;

namespace Identity.Application.EventHandlers.Users;

/// <summary>
/// Handles the UserDisabled event and disables the corresponding user from the database.
/// </summary>
public class UserDisabledEventHandler
{
    public static async Task Handle(UserDisabled userDisabled, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.LoadAsync<IdentityUser>(userDisabled.Id, cancellationToken);
        if (user is null)
        {
            logger.LogError("[UserDisabledEventHandler] User with id [{Id}] not found", userDisabled.Id);
            return;
        }

        user.Disable();
        session.Store(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}