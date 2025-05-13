using CleanIAM.Applications.Core;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace CleanIAM.Applications.Application.Queries;

/// <summary>
/// Find OpenIdClient by its Id
/// </summary>
/// <param name="Id"></param>
public record GetOpenIdApplicationByIdQuery(Guid Id);

/// <summary>
/// Query handler for `GetOpenIdClientByIdQuery`
/// </summary>
public class GetOpenIdClientByIdQueryHandler
{
    public static async Task<OpenIdApplication?> HandleAsync(GetOpenIdApplicationByIdQuery query,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager,
        CancellationToken cancellationToken)
    {
        var application = await applicationManager.FindByIdAsync(query.Id.ToString(), cancellationToken);

        var res = await OpenIdApplication.FromOpenIdDictApplication(application, applicationManager);

        return res;
    }
}