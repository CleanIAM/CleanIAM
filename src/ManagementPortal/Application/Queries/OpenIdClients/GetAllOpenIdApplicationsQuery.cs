using ManagementPortal.Core.OpenIdApplication;
using MapsterMapper;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace ManagementPortal.Application.Queries.OpenIdClients;

public record GetAllOpenIdApplicationsQuery();

public class GetAllOpenIdClientsQueryHandler
{
    public static async Task<IEnumerable<OpenIdApplication>> Handle(GetAllOpenIdApplicationsQuery query,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager, IMapper mapper)
    {
        var application = await applicationManager.ListAsync().ToArrayAsync();

        return await Task.WhenAll(
            application
                .Select(src =>
                    OpenIdApplication.FromOpenIdDictApplication(src, applicationManager))
        );

    }
}