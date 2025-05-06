using System.Net;
using ManagementPortal.Api.Controllers.Models;
using ManagementPortal.Api.Controllers.Models.Requests.Users;
using ManagementPortal.Application.Commands.Users;
using ManagementPortal.Application.Queries.Users;
using ManagementPortal.Core.Events.Users;
using ManagementPortal.Core.Users;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using SharedKernel.Core;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

/// <summary>
/// API controller for managing users
/// </summary>
[Route("/api/users")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class UsersApiController(
    IMessageBus bus) : Controller
{
    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<ApiUserModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IEnumerable<ApiUserModel>> GeAllUsersAsync(CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var res = await bus.InvokeAsync<IEnumerable<User>>(query, cancellationToken);
        return res.Adapt<IEnumerable<ApiUserModel>>();
    }

    /// <summary>
    /// Create new user
    /// </summary>
    /// <param name="request">New user data</param>
    [HttpPost]
    [ProducesResponseType<UserCreated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNewUserAsync([FromBody] CreateNewUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateNewUserCommand>();
        return await bus.InvokeAsync<Result<UserCreated>>(command, cancellationToken);
    }

    /// <summary>
    /// Get specific user by its id
    /// </summary>
    /// <param name="id">Id of the user to get</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ApiUserModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var user = await bus.InvokeAsync<User?>(query, cancellationToken);

        if (user == null)
            return Result.Error("User not found", HttpStatusCode.NotFound);
        return Result.Ok(user);
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="id">Id of user to update</param>
    /// <param name="request">New user data</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<UserUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserAsync(Guid id, UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<UpdateUserCommand>() with { Id = id };
        return await bus.InvokeAsync<Result<UserUpdated>>(command, cancellationToken);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="id">Id of user to be deleted</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType<UserDeleted>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        return await bus.InvokeAsync<Result<UserDeleted>>(command, cancellationToken);
    }

    /// <summary>
    /// Disable user
    /// </summary>
    /// <param name="id">Id of the use to disable</param>
    [ProducesResponseType<UserDisabled>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/disabled")]
    public async Task<IActionResult> DisableUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DisableUserCommand(id);
        return await bus.InvokeAsync<Result<UserDisabled>>(command, cancellationToken);
    }

    /// <summary>
    /// Enable user
    /// </summary>
    /// <param name="id">Id of the use to enable</param>
    [ProducesResponseType<UserDisabled>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:guid}/enabled")]
    public async Task<IActionResult> EnableUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new EnableUserCommand(id);
        return await bus.InvokeAsync<Result<UserEnabled>>(command, cancellationToken);
    }

    /// <summary>
    /// Invite a user to the system
    /// </summary>
    /// <param name="request">Invite user request</param>
    /// <param name="tenant">custom tenant id if superAdmin wants to invite user to different organization</param>
    /// <returns></returns>
    [HttpPost("invited")]
    [ProducesResponseType<UserInvited>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserRequest request, [FromQuery] Guid? tenant)
    {
        var command = request.Adapt<InviteUserCommand>();
        // Invoke for custom tenant only if user is super admin and custom tenant is provided
        if (tenant is not null && User.GetRoles().Contains(UserRole.SuperAdmin))
            return await bus.InvokeForTenantAsync<Result<UserInvited>>(tenant.ToString()!, command);
        return await bus.InvokeAsync<Result<UserInvited>>(command);
    }

    /// <summary>
    /// Resend user invitation email
    /// </summary>
    /// <param name="id">Id of the user</param>
    /// <param name="tenant">custom tenant id if superAdmin wants to invite user to different organization</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/invitation/email")]
    [ProducesResponseType<UserInvited>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResendInvitationEmail([FromRoute] Guid id,
        [FromQuery]
        Guid? tenant)
    {
        var command = new ResendInvitationEmailCommand(id);
        // Invoke for custom tenant only if user is super admin and custom tenant is provided
        if (tenant is not null && User.GetRoles().Contains(UserRole.SuperAdmin))
            return await bus.InvokeForTenantAsync<Result<UserInvited>>(tenant.ToString()!, command);
        return await bus.InvokeAsync<Result<UserInvited>>(command);
    }
}