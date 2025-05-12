using Events.Core.Events.Users;
using Mapster;

namespace Users.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user invited event to global event defined in Events
/// </summary>
/// <remarks>
/// Uses Wolverine's cascading messages for event propagation.
/// <a href="https://wolverinefx.net/guide/handlers/cascading.html" />
/// </remarks>
public class UserInvitedMapperHandler
{
    public static UserInvited Handle(Core.Events.Users.UserInvited localEvent, ILogger logger)
    {
        logger.LogDebug("Mapping local UserInvited event to global event");
        return localEvent.Adapt<UserInvited>();
    }
}