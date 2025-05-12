using Events.Core.Events.Identity;
using Mapster;

namespace Identity.Infrastructure.AnticorruptionLayer;

public class PasswordResetMapHandler
{
    public static PasswordReset Handle(Core.Events.PasswordReset localEvent)
    {
        return localEvent.Adapt<PasswordReset>();
    }
}