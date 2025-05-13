using CleanIAM.Events.Core.Events.Users;
using Mapster;

namespace CleanIAM.Users.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user deleted event to global event defined in CleanIAM.Events
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