using CleanIAM.Events.Core.Events.Scopes;
using Mapster;

namespace CleanIAM.Scopes.Infrastructure.AnticorruptionLayer;

/// <summary>
/// Anticorruption layer mapper form local ScopeDeleted event to global event defined in CleanIAM.Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class ScopeDeletedMapHandler
{
    public static ScopeDeleted Handle(Core.Events.ScopeDeleted localEvent)
    {
        return localEvent.Adapt<ScopeDeleted>();
    }
}