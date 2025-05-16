using System.Net;
using Mapster;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Tenants.Core;
using CleanIAM.Tenants.Core.Events;
using Wolverine;

namespace CleanIAM.Tenants.Application.Commands;

/// <summary>
/// Command to create a new tenant
/// </summary>
/// <param name="Id">Id of the new tenant</param>
/// <param name="Name">Name of the new tenant</param>
public record CreateNewTenantCommand(Guid Id, string Name);

public class CreateNewTenantCoomandHandler
{
    public static async Task<Result> LoadAsync(CreateNewTenantCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var tenant = await session.Query<Tenant>()
            .FirstOrDefaultAsync(t => t.Id == command.Id || t.Name == command.Name, token: cancellationToken);
        if (tenant is not null)
            return Result.Error("Tenant already exists", HttpStatusCode.BadRequest);

        return Result.Ok();
    }

    public static async Task<Result<NewTenantCreated>> HandleAsync(CreateNewTenantCommand command, Result loadResult,
        IDocumentSession session, IMessageBus bus, CancellationToken cancellationToken,
        ILogger<CreateNewTenantCoomandHandler> logger)
    {
        if (loadResult.IsError())
            return loadResult;

        var tenant = command.Adapt<Tenant>();
        session.Store(tenant);
        await session.SaveChangesAsync(cancellationToken);

        // Log the creation
        logger.LogInformation("Tenant {Id} created successfully", tenant.Id);

        var newTenantCreated = command.Adapt<NewTenantCreated>();
        await bus.PublishAsync(newTenantCreated);
        return Result.Ok(newTenantCreated);
    }
}