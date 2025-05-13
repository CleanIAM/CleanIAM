using CleanIAM.Scopes.Api.Controllers.Models.Requests;
using CleanIAM.Scopes.Application.Commands;
using CleanIAM.Scopes.Application.Queries;
using CleanIAM.Scopes.Core;
using CleanIAM.Scopes.Core.Events;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace CleanIAM.Scopes.Api.Controllers;

/// <summary>
/// Controller to manage OpenID scopes through API operations such as retrieval, creation, update, and deletion.
/// </summary>
[Route("/api/scopes")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Authorize(Roles = "Admin")]
public class ScopesController(IMessageBus bus) : Controller
{
    /// <summary>
    /// Get the list of all scopes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<Scope>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllScopes(CancellationToken cancellationToken)
    {
        var query = new GetAllScopesQuery();
        var scopes = await bus.InvokeAsync<IEnumerable<Scope>>(query, cancellationToken);

        return Result.Ok(scopes);
    }

    /// <summary>
    /// Get the list of default scopes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("default")]
    [ProducesResponseType<IEnumerable<Scope>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public IActionResult GetDefaultScopes(CancellationToken cancellationToken)
    {
        return Result.Ok(ScopesConstants.DefaultScopes);
    }

    /// <summary>
    /// Create a new scope.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<ScopeCreated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNewScope([FromBody] CreateNewScopeRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateNewScopeCommand>();
        return await bus.InvokeAsync<Result<ScopeCreated>>(command, cancellationToken);
    }

    /// <summary>
    /// Update a scope.
    /// </summary>
    /// <param name="scopeName">Name of the scope to update</param>
    [HttpPut("{scopeName}")]
    [ProducesResponseType<ScopeUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNewScope([FromRoute] string scopeName,
        [FromBody]
        UpdateScopeRequest request, CancellationToken cancellationToken)
    {
        var command = request.Adapt<UpdateScopeCommand>() with { Name = scopeName };
        return await bus.InvokeAsync<Result<ScopeUpdated>>(command, cancellationToken);
    }

    /// <summary>
    /// Delete a scope.
    /// </summary>
    /// <param name="scopeName">Name of the scope to delete</param>
    [HttpDelete("{scopeName}")]
    [ProducesResponseType<ScopeDeleted>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteScope([FromRoute] string scopeName, CancellationToken cancellationToken)
    {
        var command = new DeleteScopeCommand(scopeName);
        return await bus.InvokeAsync<Result<ScopeDeleted>>(command, cancellationToken);
    }
}