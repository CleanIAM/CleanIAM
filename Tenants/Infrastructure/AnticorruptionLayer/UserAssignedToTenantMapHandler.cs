using Events.Core.Events.ManagementPortal.Tenants;
using Mapster;

namespace Tenants.Infrastructure.AnticorruptionLayer;

/// <summary>
/// Anticorruption layer mapper form local UserAssignedToTenant event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class UserAssignedToTenantMapHandler
{
    public static UserAssignedToTenant Handle(Core.Events.UserAssignedToTenant localEvent)
    {
        return localEvent.Adapt<UserAssignedToTenant>();
    }
}