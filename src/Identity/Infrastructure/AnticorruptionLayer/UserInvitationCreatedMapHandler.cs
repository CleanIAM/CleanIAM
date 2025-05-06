using Events.Core.Events;
using Mapster;

namespace Identity.Infrastructure.AnticorruptionLayer;

public class UserInvitationCreatedMapHandler
{
    public static UserInvitationCreated Handle(Core.Events.UserInvitationCreated localEvent)
    {
        return localEvent.Adapt<UserInvitationCreated>();
    }
}