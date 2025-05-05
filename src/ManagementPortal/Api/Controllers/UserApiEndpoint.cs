using ManagementPortal.Api.Controllers.Models;
using ManagementPortal.Api.Controllers.Models.Requests.User;
using ManagementPortal.Application.Commands.Users;
using ManagementPortal.Application.Queries.Users;
using ManagementPortal.Core.Events.Users;
using ManagementPortal.Infrastructure.Utils;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using SharedKernel.Core.Users;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

/// <summary>
/// /// API controller for managing user with his own data
/// </summary>
/// <param name="bus"></param>
[Microsoft.AspNetCore.Components.Route("/api/user")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class UserApiEndpoint(
    IMessageBus bus) : Controller
{
    /// <summary>
    /// Get user info for the current user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType<ApiUserModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser()
    {
        var userIdRes = User.GetUserId();
        if (userIdRes.IsError())
            return userIdRes;
        var userId = userIdRes.Value;

        var query = new GetUserByIdQuery(userId);
        var result = await bus.InvokeAsync<User?>(query);

        if (result == null)
            return Result.Error("User not found", StatusCodes.Status404NotFound);

        return Ok(result.Adapt<ApiUserModel>());
    }

    /// <summary>
    /// Update user info for the current user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType<UserUpdated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserSimpleRequest request)
    {
        var userIdRes = User.GetUserId();
        if (userIdRes.IsError())
            return userIdRes;
        var userId = userIdRes.Value;

        var command = request.Adapt<UpdateUserCommand>() with { Id = userId };

        return await bus.InvokeAsync<Result<UserUpdated>>(command);
    }

    /// <summary>
    /// Enable or disable MFA for the current user
    /// </summary>
    [HttpPut("mfa/enabled")]
    public async Task<IActionResult> ToggleMfa([FromBody] UpdateMfaRequest request)
    {
        var userIdRes = User.GetUserId();
        if (userIdRes.IsError())
            return userIdRes;
        var userId = userIdRes.Value;

        var command = request.Adapt<ToggleMFAForUserCommand>() with { Id = userId };

        return await bus.InvokeAsync<Result<UserUpdated>>(command);
    }
}