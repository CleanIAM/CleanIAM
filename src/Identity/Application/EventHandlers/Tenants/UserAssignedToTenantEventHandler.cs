using Events.Core.Events.ManagementPortal.Tenants;
using Identity.Core.Users;
using Marten;

namespace Identity.Application.EventHandlers.Tenants;

public class UserAssignedToTenantEventHandler
{
    public static async Task Handle(UserAssignedToTenant @event, IDocumentSession session,
        CancellationToken cancellationToken, ILogger logger)
    {
        var user = await session.Query<IdentityUser>().Where(u => u.Id == @event.UserId && u.AnyTenant())
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            logger.LogError("[UserAssignedToTenantEventHandler] User not found: {UserId}", @event.UserId);
            return;
        }

        // Update the user's tenant information
        var oldTenantId = user.TenantId;
        user.TenantId = @event.NewTenantId;
        user.TenantName = @event.TenantName;

        // Store the user in the new tenant
        session.ForTenant(oldTenantId.ToString()).Delete(user);
        session.ForTenant(user.TenantId.ToString()).Store(user);
        await session.SaveChangesAsync(cancellationToken);
        //TODO: move other user objects in identity to new tenant
    }
}