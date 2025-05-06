using ManagementPortal.Core.Events.Users;
using Mapster;

namespace ManagementPortal.Infrastructure.AnticorruptionLayer;

/// <summary>
/// Anticorruption layer mapper form local user invited event to global event defined in Events
/// </summary>
public class UserInvitedMapperHandler
{
    public static UserInvited Handle(Events.Core.Events.UserInvited localEvent)
    {
        return localEvent.Adapt<UserInvited>();
    }
}