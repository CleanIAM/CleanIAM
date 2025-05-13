using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Tenants.Api.Controllers.Models;
using CleanIAM.Tenants.Api.Controllers.Models.Requests;
using CleanIAM.Tenants.Application.Commands;
using CleanIAM.Tenants.Application.Queries;
using CleanIAM.Tenants.Core;
using CleanIAM.Tenants.Core.Events;
using Wolverine;

namespace CleanIAM.Tenants.Api.Controllers;

/// <summary>
/// API controller for managing tenants
/// </summary>
[Route("/api/tenants")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Authorize(Roles = "MasterAdmin")]

public class TenantsController(IMessageBus bus) : Controller
{
    /// <summary>
    /// Get all tenants in the system
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<ApiTenantModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllTenantsAsync(CancellationToken cancellationToken)
    {
        var query = new GetAllTenantsQuery();
        var res = await bus.InvokeAsync<IEnumerable<Tenant>>(query, cancellationToken);
        return Result.Ok(res);
    }

    /// <summary>
    /// Get a tenant by Id
    /// </summary>
    [HttpGet("{tenantId:guid}")]
    [ProducesResponseType<ApiTenantModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTenantByIdAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var query = new GetTenantByIdQuery(tenantId);
        var res = await bus.InvokeAsync<Tenant>(query, cancellationToken);
        return Result.Ok(res);
    }

    /// <summary>
    /// Create a new tenant
    /// </summary>
    [HttpPost]
    [ProducesResponseType<NewTenantCreated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTenantAsync([FromBody] CreateNewTenantRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateNewTenantCommand>() with { Id = Guid.NewGuid() };
        return await bus.InvokeAsync<Result<NewTenantCreated>>(command, cancellationToken);
    }

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    /// <param name="tenantId">id of the tenant to update</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{tenantId:guid}")]
    [ProducesResponseType<TenantUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTenantAsync(Guid tenantId, [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<UpdateTenantCommand>() with { Id = tenantId };
        return await bus.InvokeAsync<Result<TenantUpdated>>(command, cancellationToken);
    }

    /// <summary>
    /// Assign a user to a tenant
    /// </summary>
    /// <param name="tenantId">New tenant id</param>
    /// <param name="userId">User to assign</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{tenantId:guid}/users/{userId:guid}")]
    [ProducesResponseType<UserAssignedToTenant>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignUserToTenantAsync(Guid tenantId, Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new AssignUserToTenantCommand(tenantId, userId);
        return await bus.InvokeAsync<Result<UserAssignedToTenant>>(command, cancellationToken);
    }
}