using CleanIAM.Events.Core.Events.Tenants;
using Mapster;

namespace CleanIAM.Tenants.Infrastructure.AnticorruptionLayer;

/// <summary>
/// Anticorruption layer mapper form local NewTenantCreated event to global event defined in CleanIAM.Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class NewTenantCreatedMapHandler
{
    public static NewTenantCreated Handle(Core.Events.NewTenantCreated localEvent)
    {
        return localEvent.Adapt<NewTenantCreated>();
    }
}