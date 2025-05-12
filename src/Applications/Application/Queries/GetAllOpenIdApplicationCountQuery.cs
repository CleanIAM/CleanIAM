using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Applications.Application.Queries;

public record GetAllOpenIdApplicationCountQuery;

public class GetAllOpenIdApplicationCountQueryHandler
{
    public static async Task<long> HandleAsync(GetAllOpenIdApplicationCountQuery query,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager)
    {
        return await applicationManager.CountAsync();
    }
}