using ManagementPortal.Core.Scopes;
using Mapster;
using OpenIddict.Core;
using OpenIddictScope = OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreScope<System.Guid>;

namespace ManagementPortal.Application.Queries.Scopes;

/// <summary>
/// Query to get the list of allowed scopes.
/// </summary>
public struct GetAllScopesQuery;

public class GetAllScopesQueryHandler
{
    public static async Task<IEnumerable<Scope>> Handle(GetAllScopesQuery query,
        OpenIddictScopeManager<OpenIddictScope> scopeManager)
    {
        var scopes = await scopeManager.ListAsync().ToListAsync();
        return scopes.Select(scope => scope.Adapt<Scope>())
            .ToList();
    }
}