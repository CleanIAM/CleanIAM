using ManagementPortal.Core;
using Mapster;
using OpenIddict.Abstractions;

namespace ManagementPortal.Application.Queries.OpenIdClients;

/// <summary>
/// Find OpenIdClient by its Id
/// </summary>
/// <param name="Id"></param>
public record GetOpenIdClientByIdQuery(Guid Id);

/// <summary>
/// Query handler for `GetOpenIdClientByIdQuery`
/// </summary>
public class GetOpenIdClientByIdQueryHandler
{
    public static async Task<OpenIdApplication?> Handle(GetOpenIdClientByIdQuery query, 
        IOpenIddictApplicationManager applicationManager,
        CancellationToken cancellationToken)
    {
        var application = await applicationManager.FindByIdAsync(query.Id.ToString(), cancellationToken);

        return application.Adapt<OpenIdApplication>();

    }
}