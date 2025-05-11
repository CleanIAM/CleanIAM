using Applications.Api.Controllers.Models;
using Applications.Api.Controllers.Models.Requests;
using Applications.Application.Commands;
using Applications.Application.Queries;
using Applications.Core;
using Applications.Core.Events;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace Applications.Api.Controllers;

/// <summary>
/// Controller to manage OpenID applications through API operations such as retrieval, creation, update, and deletion.
/// </summary>
[Route("/api/applications")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Authorize(Roles = "Admin")]
public class ApplicationsApiController(
    IMessageBus bus) : Controller
{
    /// <summary>
    /// Show the main application page with a list of all applications.
    /// </summary>
    [HttpGet("")]
    [ProducesResponseType<IEnumerable<ApiApplicationModel>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllApplications()
    {
        var query = new GetAllOpenIdApplicationsQuery();
        var applications = await bus.InvokeAsync<IEnumerable<OpenIdApplication>>(query);
        return Result.Ok(applications.Adapt<IEnumerable<ApiApplicationModel>>());
    }

    /// <summary>
    /// Create a new application
    /// </summary>
    /// <param name="request">New application data</param>
    /// <param name="cancellationToken"></param>
    [HttpPost]
    [ProducesResponseType<OpenIdApplicationCreated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNewApplication([FromBody] CreateNewApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateNewOpenIdApplicationCommand>() with { Id = Guid.NewGuid() };
        return await bus.InvokeAsync<Result<OpenIdApplicationCreated>>(command, cancellationToken);
    }

    /// <summary>
    /// Get the application with the given id.
    /// </summary>
    /// <param name="id">Id of the application</param>
    /// <param name="cancellationToken"></param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<OpenIdApplication>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApplication([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOpenIdApplicationByIdQuery(id);
        var application = await bus.InvokeAsync<OpenIdApplication?>(query, cancellationToken);

        if (application == null)
            return Result.Error("Application not found", StatusCodes.Status404NotFound);
        return Result.Ok(application.Adapt<ApiApplicationModel>());
    }

    /// <summary>
    /// Update the application with the given id.
    /// </summary>
    /// <param name="id">Id of the application to update</param>
    /// <param name="request">Data of the updated application</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<OpenIdApplicationUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateApplication([FromRoute] Guid id, [FromBody] UpdateApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<UpdateOpenIdApplicationCommand>() with { Id = id };
        return await bus.InvokeAsync<Result<OpenIdApplicationUpdated>>(command, cancellationToken);
    }

    /// <summary>
    /// Update the application with the given id.
    /// </summary>
    /// <param name="id">Id of the application to update</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType<OpenIdApplicationDeleted>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateApplication([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteOpenIdApplicationCommand(id);
        return await bus.InvokeAsync<Result<OpenIdApplicationDeleted>>(command, cancellationToken);
    }
}