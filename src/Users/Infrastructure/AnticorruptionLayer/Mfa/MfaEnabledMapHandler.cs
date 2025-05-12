using Events.Core.Events.Users.Mfa;
using Mapster;

namespace Users.Infrastructure.AnticorruptionLayer.Mfa;

/// <summary>
/// Anticorruption layer mapper form local MfaEnabled event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class MfaEnabledMapHandler
{
    public static MfaEnabledForUser Handle(Core.Events.Mfa.MfaEnabledForUser localEvent)
    {
        return localEvent.Adapt<MfaEnabledForUser>();
    }
}