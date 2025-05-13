using CleanIAM.Events.Core.Events.Identity;
using Mapster;

namespace CleanIAM.Identity.Infrastructure.AnticorruptionLayer;

public class NewUserSignedUpMapHandler
{
    public static NewUserSignedUp Handle(Core.Events.NewUserSignedUp localEvent)
    {
        return localEvent.Adapt<NewUserSignedUp>();
    }
}