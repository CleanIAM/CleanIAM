using Marten;
using Tenants.Core;

namespace Tenants.Application.Queries;

/// <summary>
/// Query to get a tenant by Id
/// </summary>
/// <param name="Id">Id of the tenant</param>
public record GetTenantByIdQuery(Guid Id);

public class GetTenantByIdQueryHandler
{
    public async Task<Tenant?> Handle(GetTenantByIdQuery query, IQuerySession session,
        CancellationToken cancellationToken)
    {
        return await session.LoadAsync<Tenant>(query.Id, cancellationToken);
    }
}