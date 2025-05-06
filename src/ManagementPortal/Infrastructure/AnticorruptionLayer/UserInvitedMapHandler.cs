using Events.Core.Events;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer;

/// <summary>
/// Anticorruption layer mapper form local user invited event to global event defined in Events
/// </summary>
public class UserInvitedMapperHandler
{
    public static UserInvited Handle(Core.Events.Users.UserInvited localEvent, ILogger logger)
    {
        logger.LogDebug("Mapping local UserInvited event to global event");
        return localEvent.Adapt<UserInvited>();
    }
}