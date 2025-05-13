using CleanIAM.Events.Core.Events.Users;
using Mapster;

namespace CleanIAM.Users.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user updated event to global event defined in CleanIAM.Events
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