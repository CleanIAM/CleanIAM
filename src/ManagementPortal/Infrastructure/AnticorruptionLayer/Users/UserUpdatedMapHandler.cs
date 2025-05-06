using Events.Core.Events.ManagementPortal.Users;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user updated event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class UserUpdatedMapHandler
{
    public static UserUpdated Handle(Core.Events.Users.UserUpdated localEvent)
    {
        return localEvent.Adapt<UserUpdated>();
    }
}