using CleanIAM.Events.Core.Events.Identity;
using Mapster;

namespace CleanIAM.Identity.Infrastructure.AnticorruptionLayer;

public class UserLoggedInMapHandler
{
    public static UserLoggedIn Handle(Core.Events.UserLoggedIn localEvent)
    {
        return localEvent.Adapt<UserLoggedIn>();
    }
}