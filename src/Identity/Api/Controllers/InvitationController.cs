using Identity.Api.Views.Invitation;
using Identity.Application.Commands.Invitations;
using Identity.Core.Events;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Infrastructure;
using Wolverine;

namespace Identity.Api.Controllers;

/// <summary>
/// Controller for handling user invitation flow.
/// </summary>
[Route("invitation")]
public class InvitationController(IMessageBus bus) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] Guid requestId)
    {
        return View("Invitation");
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromQuery] Guid requestId, [FromForm] InvitationViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Invitation", model);

        var command = model.Adapt<SetupUserAccountCommand>();
        var res = await bus.InvokeAsync<Result<UserAccountSetup>>(command);

        if (res.IsError())
        {
            ModelState.AddModelError("ConfirmPassword", res.ErrorValue.Message);
            return View("Invitation", model);
        }

        return View("SetupSuccess");
    }
}