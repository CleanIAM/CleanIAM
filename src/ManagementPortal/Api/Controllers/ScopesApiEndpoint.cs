using ManagementPortal.Application.Queries.Scopes;
using ManagementPortal.Core.OpenIdApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

/// <summary>
/// Controller to manage OpenID scopes through API operations such as retrieval, creation, update, and deletion.
/// </summary>
[Route("/api/scopes")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class ScopesApiEndpoint(IMessageBus bus) : Controller
{
    /// <summary>
    /// Get the list of supported scopes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<ItemWithTooltip>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllowedEndpoints(CancellationToken cancellationToken)
    {
        var query = new GetAllowedScopesQuery();
        var scopes = await bus.InvokeAsync<IEnumerable<ItemWithTooltip>>(query, cancellationToken);

        return Result.Ok(scopes);
    }
}