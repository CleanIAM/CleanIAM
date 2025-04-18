using ManagementPortal.Api.Views.Users;
using ManagementPortal.Application.Commands.Users;
using ManagementPortal.Application.Queries.Users;
using ManagementPortal.Core;
using ManagementPortal.Core.Events.Users;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

[Route("users")]
public class UsersController(
    IMessageBus bus) : Controller
{
    /// <summary>
    /// Show the main users page with a list of all users.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        var query = new GetAllUsersQuery();
        var users = await bus.InvokeAsync<IEnumerable<User>>(query);

        return View(new UserPageModel { Users = users.ToArray() });
    }

    /// <summary>
    /// Get form for creating new user
    /// </summary>
    /// <returns></returns>
    [HttpGet("new")]
    public IActionResult CreateNewUserAsync()
    {
        return View();
    }

    /// <summary>
    /// Handle form submission for creating new user
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("new")]
    public async Task<IActionResult> CreateNewUserAsync([FromForm] CreateNewUserModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var command = model.Adapt<CreateNewUserCommand>() with { Id = Guid.NewGuid() };
        var res = await bus.InvokeAsync<Result<UserCreated>>(command, cancellationToken);

        if (res.IsError())
        {
            ModelState.AddModelError("", res.ErrorValue.Message);
            return View(model);
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Show the edit page for a specific user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> EditUserAsync([FromRoute] Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var user = await bus.InvokeAsync<User?>(query);

        if (user is null)
            return NotFound();

        var model = user.Adapt<EditUserModel>();

        return View(model);
    }

    /// <summary>
    /// Update the user with the given id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> EditPostAsync([FromRoute] Guid id, EditUserModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("EditUser", model);
        }

        var command = model.Adapt<UpdateUserCommand>();
        var result = await bus.InvokeAsync<Result<UserUpdated>>(command);
        if (result.IsError())
        {
            ModelState.AddModelError("", result.ErrorValue.Message);
            return View("EditUser", model);
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// HTMX endpoint to delete a user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        var command = new DeleteUserCommand(id);
        var result = await bus.InvokeAsync<Result<UserDeleted>>(command);
        if (result.IsError())
        {
            return BadRequest(result.ErrorValue.Message);
        }

        return Ok();
    }
}