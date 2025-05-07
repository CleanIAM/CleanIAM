using Identity.Api.Views.Invitation;
using Identity.Application.Commands.Invitations;
using Identity.Core.Events;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace Identity.Api.Controllers;

/// <summary>
/// Controller for handling user invitation flow.
/// </summary>
[Route("/invitations")]
public class InvitationController(IMessageBus bus) : Controller
{
    [HttpGet("{requestId:guid}")]
    public async Task<IActionResult> Index([FromRoute] Guid requestId)
    {
        return View("Invitation", new InvitationViewModel { RequestId = requestId });
    }

    [HttpPost("{requestId:guid}")]
    public async Task<IActionResult> Index([FromRoute] Guid requestId, [FromForm] InvitationViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Invitation", model);

        var command = new SetupUserAccountCommand(requestId, model.NewPassword);
        var res = await bus.InvokeAsync<Result<UserAccountSetup>>(command);

        if (res.IsError())
        {
            ModelState.AddModelError("ConfirmPassword", res.ErrorValue.Message);
            return View("Invitation", model);
        }

        return View("SetupSuccess");
    }
}