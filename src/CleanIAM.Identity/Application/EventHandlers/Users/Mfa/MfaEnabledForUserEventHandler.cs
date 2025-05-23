using CleanIAM.Events.Core.Events.Users.Mfa;
using CleanIAM.Identity.Core.Users;
using Marten;

namespace CleanIAM.Identity.Application.EventHandlers.Users.Mfa;

public class MfaEnabledForUserEventHandler
{
    public static async Task HandleAsync(MfaEnabledForUser mfaEnabled, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.LoadAsync<IdentityUser>(mfaEnabled.Id, cancellationToken);
        if (user is null)
        {
            logger.LogError($"[MfaEnabledForUserEventHandler] Could not find user {mfaEnabled.Id}");
            return;
        }

        // Update the user in database
        user.IsMFAEnabled = true;
        session.Update(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}