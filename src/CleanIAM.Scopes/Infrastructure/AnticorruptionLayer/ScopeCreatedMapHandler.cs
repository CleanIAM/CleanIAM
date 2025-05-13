using CleanIAM.Events.Core.Events.Scopes;
using Mapster;

namespace CleanIAM.Scopes.Infrastructure.AnticorruptionLayer;

/// <summary>
/// Anticorruption layer mapper form local ScopeCreated event to global event defined in CleanIAM.Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class ScopeCreatedMapHandler
{
    public static ScopeCreated Handle(Core.Events.ScopeCreated localEvent)
    {
        return localEvent.Adapt<ScopeCreated>();
    }
}