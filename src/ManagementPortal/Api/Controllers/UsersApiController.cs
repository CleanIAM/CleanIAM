using System.Net;
using ManagementPortal.Api.Controllers.Models;
using ManagementPortal.Api.Controllers.Models.Requests.Users;
using ManagementPortal.Application.Commands.Users;
using ManagementPortal.Application.Queries.Users;
using ManagementPortal.Core;
using ManagementPortal.Core.Events.Users;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

/// <summary>
/// API controller for managing users
/// </summary>
[Route("/api/users")]
public class UsersApiController(
    IMessageBus bus) : Controller
{
    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns></returns>
    [HttpGet("")]
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
    /// <returns></returns>
    [HttpPost("")]
    [ProducesResponseType<UserCreated>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNewUserAsync(CreateNewUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<CreateNewUserCommand>();
        return await bus.InvokeAsync<Result<UserCreated>>(command, cancellationToken);
    }

    /// <summary>
    /// Get specific user by its id
    /// </summary>
    /// <param name="id">Id of the user to get</param>
    /// <returns></returns>
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
    /// <returns></returns>
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
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType<UserDeleted>(StatusCodes.Status200OK)]
    [ProducesResponseType<Error>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Error>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        return await bus.InvokeAsync<Result<UserDeleted>>(command, cancellationToken);
    }
}