using Events.Core.Events.ManagementPortal.Scopes;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer.Scopes;

/// <summary>
/// Anticorruption layer mapper form local ScopeDeleted event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class ScopeDeletedMapHandler
{
    public static ScopeDeleted Handle(Core.Events.Scopes.ScopeDeleted localEvent)
    {
        return localEvent.Adapt<ScopeDeleted>();
    }
}