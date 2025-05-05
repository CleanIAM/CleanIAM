using ManagementPortal.Core;
using Marten;

namespace ManagementPortal.Application.Queries.Tenants;

public record GetAllTenantsQuery;

public class GetAllTenantsQueryHandler
{
    public async Task<IEnumerable<Tenant>> Handle(GetAllTenantsQuery query, IQuerySession session,
        CancellationToken cancellationToken)
    {
        return await session.Query<Tenant>().ToListAsync(cancellationToken);
    }
}