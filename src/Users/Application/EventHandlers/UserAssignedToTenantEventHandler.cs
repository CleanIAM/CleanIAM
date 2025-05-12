using Events.Core.Events.Tenants;
using Marten;
using Users.Core;

namespace Users.Application.EventHandlers;

public class UserAssignedToTenantEventHandler
{
    public static async Task HandleAsync(UserAssignedToTenant userAssignedEvent, IDocumentSession session,
        ILogger logger, CancellationToken cancellationToken)
    {
        var user = await session.Query<User>().Where(u => u.Id == userAssignedEvent.UserId && u.AnyTenant())
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            logger.LogError("[UserAssignedToTenantEventHandler] User not found: {UserId}", userAssignedEvent.UserId);
            return;
        }

        // Update the user's tenant information
        var oldTenantId = user.TenantId;
        user.TenantId = userAssignedEvent.NewTenantId;
        user.TenantName = userAssignedEvent.TenantName;

        // Store the user in the new tenant
        session.ForTenant(oldTenantId.ToString()).Delete(user);
        session.ForTenant(user.TenantId.ToString()).Store(user);
        await session.SaveChangesAsync(cancellationToken);
    }
}