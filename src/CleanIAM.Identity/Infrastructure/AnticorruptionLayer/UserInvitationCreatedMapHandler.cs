using CleanIAM.Events.Core.Events.Users;
using Mapster;

namespace CleanIAM.Identity.Infrastructure.AnticorruptionLayer;

public class UserInvitationCreatedMapHandler
{
    public static UserInvitationCreated Handle(Core.Events.UserInvitationCreated localEvent)
    {
        return localEvent.Adapt<UserInvitationCreated>();
    }
}