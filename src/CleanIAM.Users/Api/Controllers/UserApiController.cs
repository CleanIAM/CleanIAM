using CleanIAM.Events.Core.Events.Users.Mfa;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using CleanIAM.SharedKernel.Infrastructure;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Users.Api.Controllers.Models;
using CleanIAM.Users.Api.Controllers.Models.Requests.Mfa;
using CleanIAM.Users.Api.Controllers.Models.Requests.User;
using CleanIAM.Users.Api.Controllers.Models.Responses.Mfa;
using CleanIAM.Users.Application.Commands.Mfa;
using CleanIAM.Users.Application.Commands.Users;
using CleanIAM.Users.Application.Queries.Users;
using CleanIAM.Users.Core;
using CleanIAM.Users.Core.Events.Users;
using Wolverine;

namespace CleanIAM.Users.Api.Controllers;

/// <summary>
/// /// API controller for managing user with his own data
/// </summary>
/// <remarks>No specific roles required</remarks>
[Route("/api/user")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class UserApiController(
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

        var command = request.Adapt<UpdateUserSimpleCommand>() with { Id = userId };

        return await bus.InvokeAsync<Result<UserUpdated>>(command);
    }

    /// <summary>
    /// Enable or disable MFA for the current user
    /// </summary>
    [HttpPut("mfa/enabled")]
    [ProducesResponseType<MfaUpdatedResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ToggleMfa([FromBody] EnableMfaRequest request)
    {
        var userIdRes = User.GetUserId();
        if (userIdRes.IsError())
            return userIdRes;
        var userId = userIdRes.Value;

        if (request.Enable)
        {
            var command = new EnableMfaForUserCommand(userId);
            var res = await bus.InvokeAsync<Result<MfaEnabledForUser>>(command);
            if (res.IsError())
                return res;
            return Result.Ok(new MfaUpdatedResponse
            {
                MfaEnabled = true
            });
        }
        else
        {
            var command = new DisableMfaForUserCommand(userId);
            var res = await bus.InvokeAsync<Result<MfaDisabledForUser>>(command);
            if (res.IsError())
                return res;
            return Result.Ok(new MfaUpdatedResponse
            {
                MfaEnabled = false
            });
        }
    }

    /// <summary>
    /// Generate a QR code for MFA connection
    /// </summary>
    /// <returns>The qrcode image encoded in base64</returns>
    [HttpGet("mfa/configuration")]
    [ProducesResponseType<MfaConfigurationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMfaQrCode()
    {
        var userIdRes = User.GetUserId();
        if (userIdRes.IsError())
            return userIdRes;
        var userId = userIdRes.Value;

        var command = new GenerateMfaConnectionQrCodeCommand(userId);
        var res = await bus.InvokeAsync<Result<string>>(command);

        if (res.IsError())
            return res;
        return Result.Ok(new MfaConfigurationResponse
        {
            QrCode = res.Value
        });
    }

    /// <summary>
    /// Validate Totp and if valid, configure MFA for the user
    /// </summary>
    [HttpPost("mfa/configuration")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfigureMfa([FromBody] ConfigureMfaRequest request)
    {
        var userIdRes = User.GetUserId();
        if (userIdRes.IsError())
            return userIdRes;
        var userId = userIdRes.Value;

        var command = request.Adapt<ValidateMfaConnectionCommand>() with { Id = userId };
        var res = await bus.InvokeAsync<Result<MfaConfiguredForUser>>(command);

        if (res.IsError())
            return res;
        // Do not return MfaConfiguredForUser event since it contains sensitive data (TotpSecretKey)
        return Result.Ok();
    }

    /// <summary>
    /// Remove mfa configuration command for user and disable mfa.
    /// </summary>
    [HttpDelete("mfa/configuration")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CleanMfa()
    {
        var userIdRes = User.GetUserId();
        if (userIdRes.IsError())
            return userIdRes;
        var userId = userIdRes.Value;

        var command = new CleanMfaConfigurationCommand(userId);
        var res = await bus.InvokeAsync<Result<MfaConfiguredForUser>>(command);

        if (res.IsError())
            return res;
        // Do not return MfaConfiguredForUser event since it contains sensitive data (TotpSecretKey)
        return Result.Ok();
    }
}