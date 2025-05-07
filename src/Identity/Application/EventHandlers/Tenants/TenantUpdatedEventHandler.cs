using Events.Core.Events.ManagementPortal.Tenants;
using Identity.Core.Users;
using Marten;

namespace Identity.Application.EventHandlers.Tenants;

public class TenantUpdatedEventHandler
{
    public static async Task Handle(TenantUpdated tenantUpdated, IDocumentSession session,
        CancellationToken cancellationToken)
    {
        // Query all affected users
        var users = await session.Query<IdentityUser>()
            .Where(user => user.TenantId == tenantUpdated.Id && user.AnyTenant())
            .ToListAsync(cancellationToken);

        // Update users
        foreach (var user in users)
            user.TenantName = tenantUpdated.Name;

        // Store updated users
        session.Update(users.ToArray());
        await session.SaveChangesAsync(cancellationToken);
    }
}