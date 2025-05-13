using CleanIAM.Events.Core.Events.Identity;
using Mapster;

namespace CleanIAM.Identity.Infrastructure.AnticorruptionLayer;

public class UserEmailVerifiedMapHandler
{
    public static UserEmailVerified Handle(Core.Events.UserEmailVerified localEvent)
    {
        return localEvent.Adapt<UserEmailVerified>();
    }
}