using CleanIAM.Events.Core.Events.Users.Mfa;
using Mapster;

namespace CleanIAM.Users.Infrastructure.AnticorruptionLayer.Mfa;

/// <summary>
/// Anticorruption layer mapper form local MfaConfigured event to global event defined in CleanIAM.Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class MfaConfiguredMapHandler
{
    public static MfaConfiguredForUser Handle(Core.Events.Mfa.MfaConfiguredForUser localEvent)
    {
        return localEvent.Adapt<MfaConfiguredForUser>();
    }
}