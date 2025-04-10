using ManagementPortal.Core;
using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using MapsterMapper;
using OpenIddict.Abstractions;

namespace ManagementPortal.Application.Queries.OpenIdClients;

public record GetAllOpenIdClientsQuery();

public class GetAllOpenIdClientsQueryHandler
{
    public static async Task<IEnumerable<OpenIdApplication>> Handle(GetAllOpenIdClientsQuery query,
        IOpenIddictApplicationManager applicationManager, IMapper mapper)
    {
        return await mapper.From(await applicationManager.ListAsync().ToArrayAsync()).AdaptToTypeAsync<OpenIdApplication[]>();
    }
}