using System.Net;
using Mapster;
using Marten;
using SharedKernel.Infrastructure.Utils;
using Tenants.Core;
using Tenants.Core.Events;
using Wolverine;

namespace Tenants.Application.Commands;

/// <summary>
/// Command to assign a user to a tenant
/// </summary>
/// <param name="NewTenantId">New tenant id</param>
/// <param name="UserId">user id</param>
public record AssignUserToTenantCommand(Guid NewTenantId, Guid UserId);

public class AssignUsersToTenantCommandHandler
{
    //TODO: FIX this

    public async Task<Result<Tenant>> LoadAsync(AssignUserToTenantCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var tenant = await session.LoadAsync<Tenant>(command.NewTenantId, cancellationToken);
        if (tenant is null)
            return Result.Error("Tenant not found", HttpStatusCode.NotFound);

        return Result.Ok(tenant);
    }

    public async Task<Result<UserAssignedToTenant>> Handle(AssignUserToTenantCommand command,
        Result<Tenant> loadResult, IDocumentSession session, IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.Error(loadResult.ErrorValue);
        var tenant = loadResult.Value;

        var userAssignedToTenant = command.Adapt<UserAssignedToTenant>() with { TenantName = tenant.Name };
        await bus.PublishAsync(userAssignedToTenant);
        return Result.Ok(userAssignedToTenant);
    }
}