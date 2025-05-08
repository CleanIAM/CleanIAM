using Events.Core.Events.ManagementPortal.Tenants;
using Mapster;

namespace Tenants.Infrastructure.AnticorruptionLayer;

/// <summary>
/// Anticorruption layer mapper form local TenantUpdated event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class TenantUpdatedMapHandler
{
    public static TenantUpdated Handle(Core.Events.TenantUpdated localEvent)
    {
        return localEvent.Adapt<TenantUpdated>();
    }
}