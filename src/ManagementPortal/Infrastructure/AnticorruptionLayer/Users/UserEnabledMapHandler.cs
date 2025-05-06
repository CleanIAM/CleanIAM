using Events.Core.Events.ManagementPortal.Users;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user enabled event to global event defined in Events
/// </summary>
public class UserEnabledMapHandler
{
    public static UserEnabled Handle(Core.Events.Users.UserEnabled localEvent)
    {
        return localEvent.Adapt<UserEnabled>();
    }
}