using Events.Core.Events.ManagementPortal.Users;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user deleted event to global event defined in Events
/// </summary>
public class UserDeletedMapHandler
{
    public static UserDeleted Handle(Core.Events.Users.UserDeleted localEvent)
    {
        return localEvent.Adapt<UserDeleted>();
    }
}