using Events.Core.Events.Identity;
using Mapster;

namespace Identity.Infrastructure.AnticorruptionLayer;

public class UserEmailVerifiedMapHandler
{
    public static UserEmailVerified Handle(Core.Events.UserEmailVerified localEvent)
    {
        return localEvent.Adapt<UserEmailVerified>();
    }
}