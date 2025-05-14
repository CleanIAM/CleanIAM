using System.Net;
using CleanIAM.SharedKernel;
using Mapster;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Tenants.Core;
using CleanIAM.Tenants.Core.Events;
using Wolverine;

namespace CleanIAM.Tenants.Application.Commands;

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

        if (tenant.Id == SharedKernelConstants.DefaultTenantId)
            return Result.Error("Default tenant cannot be updated", HttpStatusCode.Forbidden);

        return Result.Ok(tenant);
    }

    public static async Task<Result<TenantUpdated>> HandleAsync(UpdateTenantCommand command, Result<Tenant> loadResult,
        IDocumentSession session, IMessageBus bus, CancellationToken cancellationToken,
        ILogger<UpdateTenantCommandHandler> logger)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var tenant = loadResult.Value;

        // Update tenant
        tenant.Name = command.Name;
        session.Store(tenant);
        await session.SaveChangesAsync(cancellationToken);

        // Log the update
        logger.LogInformation("Tenant {Id} updated successfully", tenant.Id);

        var tenantUpdated = command.Adapt<TenantUpdated>();
        await bus.PublishAsync(tenantUpdated);
        return Result.Ok(tenantUpdated);
    }
}