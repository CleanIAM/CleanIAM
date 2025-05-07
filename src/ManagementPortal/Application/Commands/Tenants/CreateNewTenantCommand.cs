using System.Net;
using ManagementPortal.Core;
using ManagementPortal.Core.Events.Tenants;
using Mapster;
using Marten;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace ManagementPortal.Application.Commands.Tenants;

/// <summary>
/// Command to create a new tenant
/// </summary>
/// <param name="Id">Id of the new tenant</param>
/// <param name="Name">Name of the new tenant</param>
public record CreateNewTenantCommand(Guid Id, string Name);

public class CreateNewTenantCoomandHandler
{
    public async Task<Result> LoadAsync(CreateNewTenantCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var tenant = session.Query<Tenant>().FirstOrDefault(t => t.Id == command.Id || t.Name == command.Name);
        if (tenant is not null)
            return Result.Error("Tenant already exists", HttpStatusCode.BadRequest);

        return Result.Ok();
    }

    public async Task<Result<NewTenantCreated>> Handle(CreateNewTenantCommand command, Result loadResult,
        IDocumentSession session, IMessageBus bus, CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return loadResult;

        var tenant = command.Adapt<Tenant>();
        session.Store(tenant);
        await session.SaveChangesAsync(cancellationToken);

        var newTenantCreated = command.Adapt<NewTenantCreated>();
        await bus.PublishAsync(newTenantCreated);
        return Result.Ok(newTenantCreated);
    }
}