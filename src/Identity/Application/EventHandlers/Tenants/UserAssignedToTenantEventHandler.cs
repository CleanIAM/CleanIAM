using Events.Core.Events.ManagementPortal.Tenants;
using Identity.Core.Users;
using Marten;

namespace Identity.Application.EventHandlers.Tenants;

public class UserAssignedToTenantEventHandler
{
    public static async Task Handle(UserAssignedToTenant @event, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.Query<IdentityUser>()
            .Where(user => user.Id == @event.UserId && user.AnyTenant())
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            logger.LogError(
                $"[UserAssignedToTenantEventHandler] User with id [{@event.UserId}] not found");
            return;
        }

        // Update user
        user.TenantId = @event.NewTenantId;
        user.TenantName = @event.TenantName;
        session.ForTenant(@event.OldTenantId.ToString()).Delete<IdentityUser>(user.Id);
        session.ForTenant(@event.NewTenantId.ToString()).Store(user);

        //TODO: move other user objects in identity to new tenant
    }
}