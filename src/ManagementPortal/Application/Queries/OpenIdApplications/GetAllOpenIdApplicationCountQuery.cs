using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace ManagementPortal.Application.Queries.OpenIdApplications;

public record GetAllOpenIdApplicationCountQuery();

public class GetAllOpenIdApplicationCountQueryHandler
{
    public static async Task<long> Handle(GetAllOpenIdApplicationCountQuery query,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager)
    {
        return await applicationManager.CountAsync();
    }
}