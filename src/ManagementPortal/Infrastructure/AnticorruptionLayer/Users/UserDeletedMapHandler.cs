using Events.Core.Events.ManagementPortal.Users;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user deleted event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class UserDeletedMapHandler
{
    public static UserDeleted Handle(Core.Events.Users.UserDeleted localEvent)
    {
        return localEvent.Adapt<UserDeleted>();
    }
}