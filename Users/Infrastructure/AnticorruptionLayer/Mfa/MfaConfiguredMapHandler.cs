using Events.Core.Events.ManagementPortal.Mfa;
using Mapster;

namespace Users.Infrastructure.AnticorruptionLayer.Mfa;

/// <summary>
/// Anticorruption layer mapper form local MfaConfigured event to global event defined in Events
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