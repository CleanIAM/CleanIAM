using CleanIAM.Events.Core.Events.Users.Mfa;
using CleanIAM.Identity.Core.Users;
using Marten;

namespace CleanIAM.Identity.Application.EventHandlers.Users.Mfa;

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