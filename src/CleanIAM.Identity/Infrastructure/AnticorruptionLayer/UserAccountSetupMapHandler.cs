using CleanIAM.Events.Core.Events;
using CleanIAM.Events.Core.Events.Identity;
using Mapster;

namespace CleanIAM.Identity.Infrastructure.AnticorruptionLayer;

public class UserAccountSetupMapHandler
{
    public static UserAccountSetup Handle(Core.Events.UserAccountSetup localEvent)
    {
        return localEvent.Adapt<UserAccountSetup>();
    }
}