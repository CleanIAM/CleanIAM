using ManagementPortal.Api.Controllers.Models;
using ManagementPortal.Api.Controllers.Models.Requests.Tenants;
using ManagementPortal.Application.Commands.Tenants;
using ManagementPortal.Application.Queries.Tenants;
using ManagementPortal.Core;
using ManagementPortal.Core.Events.Tenants;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

/// <summary>
/// API controller for managing tenants
/// </summary>
[Route("/api/tenants")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
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