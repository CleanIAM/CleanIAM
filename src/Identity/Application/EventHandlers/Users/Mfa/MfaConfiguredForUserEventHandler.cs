using Events.Core.Events.ManagementPortal.Mfa;
using Identity.Core.Users;
using Marten;

namespace Identity.Application.EventHandlers.Users.Mfa;

public class MfaConfiguredForUserEventHandler
{
    public static async Task HandleAsync(MfaConfiguredForUser mfaConfigured, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.LoadAsync<IdentityUser>(mfaConfigured.Id, cancellationToken);
        if (user is null)
        {
            logger.LogError($"[MfaConfiguredForUserEventHandler] Could not find user {mfaConfigured.Id}");
            return;
        }

        // Update the user with MFA configuration
        user.MfaConfig.IsMfaConfigured = true;
        user.MfaConfig.TotpSecretKey = mfaConfigured.TotpSecretKey;
        user.IsMFAEnabled = mfaConfigured.MfaEnabled;
        session.Update(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}