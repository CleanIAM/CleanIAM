using System.Net;
using ManagementPortal.Core;
using ManagementPortal.Core.Events.Tenants;
using ManagementPortal.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace ManagementPortal.Application.Commands.Tenants;

/// <summary>
/// Command to update an existing tenant
/// </summary>
/// <param name="Id">Id of the existing tenant</param>
/// <param name="Name">New name for the tenant</param>
public record UpdateTenantCommand(Guid Id, string Name);

public class UpdateTenantCommandHandler
{
    public static async Task<Result<Tenant>> LoadAsync(UpdateTenantCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var tenant = await session.LoadAsync<Tenant>(command.Id, cancellationToken);
        if (tenant is null)
            return Result.Error("Tenant not found", HttpStatusCode.NotFound);

        return Result.Ok(tenant);
    }

    public static async Task<Result<TenantUpdated>> Handle(UpdateTenantCommand command, Result<Tenant> loadResult,
        IDocumentSession session, IMessageBus bus, CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var tenant = loadResult.Value;

        // Update tenant
        tenant.Name = command.Name;
        session.Store(tenant);

        // Query all affected users
        var users = await session.Query<User>()
            .Where(user => user.TenantId == command.Id && user.AnyTenant())
            .ToListAsync(cancellationToken);

        // Update users
        foreach (var user in users)
            user.TenantName = command.Name;

        session.Update(users.ToArray());

        await session.SaveChangesAsync(cancellationToken);

        var tenantUpdated = command.Adapt<TenantUpdated>();
        await bus.PublishAsync(tenantUpdated);
        return Result.Ok(tenantUpdated);
    }
}