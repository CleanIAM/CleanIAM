using System.Net;
using CleanIAM.Events.Core.Events.Users.Mfa;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Users.Api.Controllers.Models;
using CleanIAM.Users.Api.Controllers.Models.Requests.Users;
using CleanIAM.Users.Application.Commands.Mfa;
using CleanIAM.Users.Application.Commands.Users;
using CleanIAM.Users.Application.Queries.Users;
using CleanIAM.Users.Core;
using CleanIAM.Users.Core.Events.Users;
using Wolverine;

namespace CleanIAM.Users.Api.Controllers;

/// <summary>
/// API controller for managing users
/// </summary>
[Route("/api/users")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Authorize(Roles = "Admin,MasterAdmin")]
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
    public async Task<IActionResult> UpdateUserAsync(Guid id, [FromBody] UpdateUserRequest request,
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
    [HttpPut("{id:guid}/disabled")]
    [ProducesResponseType<UserDisabled>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DisableUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DisableUserCommand(id);
        return await bus.InvokeAsync<Result<UserDisabled>>(command, cancellationToken);
    }

    /// <summary>
    /// Enable user
    /// </summary>
    /// <param name="id">Id of the use to enable</param>
    [HttpPut("{id:guid}/enabled")]
    [ProducesResponseType<UserDisabled>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
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
    public async Task<IActionResult> InviteUser([FromBody] InviteUserRequest request)
    {
        var command = request.Adapt<InviteUserCommand>();
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
    public async Task<IActionResult> ResendInvitationEmail([FromRoute] Guid id)
    {
        var command = new ResendInvitationEmailCommand(id);
        return await bus.InvokeAsync<Result<UserInvited>>(command);
    }

    /// <summary>
    /// Disable MFA for user
    /// </summary>
    /// <param name="id">Id of user to disable mfa for</param>
    /// <returns></returns>
    [HttpDelete("{id:guid}/mfa/enabled")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DisableMfaForUser([FromRoute] Guid id)
    {
        var command = new CleanMfaConfigurationCommand(id);
        return await bus.InvokeAsync<Result>(command);
    }
}