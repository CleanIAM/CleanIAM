using System.Net;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Tenants.Core;
using Marten;

namespace CleanIAM.Tenants.Application.Commands;

/// <summary>
/// Command to delete a tenant
/// </summary>
/// <param name="Id">Id of the tenant to be deleted</param>
public record DeleteTenantCommand(Guid Id);

public class DeleteTenantCommandHandler
{
    public static async Task<Result> HandleAsync(DeleteTenantCommand command, IDocumentStore store, IDocumentSession documentSession, CancellationToken cancellationToken)
    {
        var session = store.QuerySession();
        
        var tenant = await session.LoadAsync<Tenant>(command.Id, cancellationToken);
        if (tenant is null)
            return Result.Error("Tenant not found", HttpStatusCode.NotFound);

        // Delete the tenant
        documentSession.Delete(tenant);
        await documentSession.SaveChangesAsync(cancellationToken);

        // Delete all tenanted data
        await store.Advanced.DeleteAllTenantDataAsync("AAA", cancellationToken);
        
        return Result.Ok();
    }
}

