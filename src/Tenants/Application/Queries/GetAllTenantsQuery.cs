using Marten;
using Tenants.Core;

namespace Tenants.Application.Queries;

public record GetAllTenantsQuery;

public class GetAllTenantsQueryHandler
{
    public async Task<IEnumerable<Tenant>> Handle(GetAllTenantsQuery query, IQuerySession session,
        CancellationToken cancellationToken)
    {
        return await session.Query<Tenant>().Where(t => t.AnyTenant()).ToListAsync(cancellationToken);
    }
}