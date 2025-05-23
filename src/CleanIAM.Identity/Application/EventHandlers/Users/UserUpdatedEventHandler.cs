using CleanIAM.Events.Core.Events.Users;
using CleanIAM.Identity.Core.Users;
using Mapster;
using Marten;

namespace CleanIAM.Identity.Application.EventHandlers.Users;

public class UserUpdatedEventHandler
{
    public static async Task Handle(UserUpdated userUpdated, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.LoadAsync<IdentityUser>(userUpdated.Id, cancellationToken);
        if (user is null)
        {
            logger.LogError("[UserUpdatedEventHandler] User with id [{Id}] not found", userUpdated.Id);
            return;
        }

        user = userUpdated.Adapt(user);
        session.Update(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}