using Events.Core.Events.Users;
using Mapster;

namespace Users.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user enabled event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class UserEnabledMapHandler
{
    public static UserEnabled Handle(Core.Events.Users.UserEnabled localEvent)
    {
        return localEvent.Adapt<UserEnabled>();
    }
}