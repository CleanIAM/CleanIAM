using Events.Core.Events.ManagementPortal.Mfa;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer.Mfa;

/// <summary>
/// Anticorruption layer mapper form local MfaDisabled event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class MfaDisabledMapHandler
{
    public MfaDisabledForUser Handle(Core.Events.Mfa.MfaDisabledForUser localEvent)
    {
        return localEvent.Adapt<MfaDisabledForUser>();
    }
}