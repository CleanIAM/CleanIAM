using System.Net;
using ManagementPortal.Core;
using ManagementPortal.Core.Events.Tenants;
using ManagementPortal.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Core.Users;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.Tenants;

/// <summary>
/// Command to assign a user to a tenant
/// </summary>
/// <param name="TenantId">New tenant id</param>
/// <param name="UserId">user id</param>
public record AssignUserToTenantCommand(Guid TenantId, Guid UserId);

public class AssignUsersToTenantCommandHandler
{
    public async Task<Result<(User, Tenant)>> LoadAsync(AssignUserToTenantCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var tenant = await session.LoadAsync<Tenant>(command.TenantId, cancellationToken);
        if (tenant is null)
            return Result.Error("Tenant not found", HttpStatusCode.NotFound);

        var user = await session.LoadAsync<User>(command.UserId, cancellationToken);
        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok((user, tenant));
    }

    public async Task<Result<UserAssignedToTenant>> Handle(AssignUserToTenantCommand command,
        Result<(User, Tenant)> loadResult, IDocumentSession session, IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.Error(loadResult.ErrorValue);
        var (user, tenant) = loadResult.Value;

        session.ForTenant(user.TenantId.ToString()).Delete(user);
        session.ForTenant(command.TenantId.ToString()).Store(user);
        await session.SaveChangesAsync(cancellationToken);

        var userAssignedToTenant = command.Adapt<UserAssignedToTenant>() with
        {
            TenantName = tenant.Name, UserFirstName = user.FirstName, UserLastName = user.LastName
        };
        await bus.PublishAsync(userAssignedToTenant);
        return Result.Ok(userAssignedToTenant);
    }
}