using CleanIAM.Tenants.Core;
using Marten;

namespace CleanIAM.Tenants.Application.Queries;

/// <summary>
/// Query to get a tenant by Id
/// </summary>
/// <param name="Id">Id of the tenant</param>
public record GetTenantByIdQuery(Guid Id);

public class GetTenantByIdQueryHandler
{
    public static async Task<Tenant?> HandleAsync(GetTenantByIdQuery query, IQuerySession session,
        CancellationToken cancellationToken)
    {
        return await session.LoadAsync<Tenant>(query.Id, cancellationToken);
    }
}