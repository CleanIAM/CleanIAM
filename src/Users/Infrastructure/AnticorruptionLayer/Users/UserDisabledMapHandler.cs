using Events.Core.Events.ManagementPortal.Users;
using Mapster;

namespace Users.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user disabled event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class UserDisabledMapHandler
{
    public static UserDisabled Handle(Core.Events.Users.UserDisabled localEvent)
    {
        return localEvent.Adapt<UserDisabled>();
    }
}