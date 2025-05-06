using Events.Core.Events.ManagementPortal.Users;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer.Users;

/// <summary>
/// Anticorruption layer mapper form local user updated event to global event defined in Events
/// </summary>
public class UserUpdatedMapHandler
{
    public static UserUpdated Handle(Core.Events.Users.UserUpdated localEvent)
    {
        return localEvent.Adapt<UserUpdated>();
    }
}