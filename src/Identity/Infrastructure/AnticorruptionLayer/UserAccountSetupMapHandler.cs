using Events.Core.Events;
using Events.Core.Events.Identity;
using Mapster;

namespace Identity.Infrastructure.AnticorruptionLayer;

public class UserAccountSetupMapHandler
{
    public static UserAccountSetup Handle(Core.Events.UserAccountSetup localEvent)
    {
        return localEvent.Adapt<UserAccountSetup>();
    }
}