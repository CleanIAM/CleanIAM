using ManagementPortal.Core.OpenIdApplication;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace ManagementPortal.Application.Queries.OpenIdApplications;

public struct GetAllowedScopesQuery;

public class GetAllowedScopesQueryHandler
{
    public static async Task<IEnumerable<ItemWithTooltip>> Handle(GetAllowedScopesQuery query,
        OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope<Guid>> scopeManager)
    {
        var scopes = await scopeManager.ListAsync().ToListAsync();
        return scopes.Select(scope => new ItemWithTooltip{ Value = scope.Name?? "Unknown scope", Tooltip = $"Resources: {scope.Resources}" })
            .OrderBy(scope => scope.Value)
            .ToList();
    }
};
