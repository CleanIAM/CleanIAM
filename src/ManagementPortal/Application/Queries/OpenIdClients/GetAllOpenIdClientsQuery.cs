using ManagementPortal.Core;
using Mapster;
using OpenIddict.Abstractions;

namespace ManagementPortal.Application.Queries.OpenIdClients;

public record GetAllOpenIdClientsQuery();

public class GetAllOpenIdClientsQueryHandler
{
    public static async Task<IEnumerable<OpeinIdClient>> Handle(GetAllOpenIdClientsQuery query,
        IOpenIddictApplicationManager applicationManager)
    {
        return (await applicationManager.ListAsync().ToArrayAsync())
            .Adapt<OpeinIdClient[]>();
    }
}