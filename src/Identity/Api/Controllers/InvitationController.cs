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
    /// <summary>
    /// Show the invitation page for a user to set up their account
    /// </summary>
    /// <param name="requestId">Id og te signin request</param>
    [HttpGet("{requestId:guid}")]
    public IActionResult Index([FromRoute] Guid requestId)
    {
        return View("Invitation", new InvitationViewModel { RequestId = requestId });
    }

    /// <summary>
    /// Handle the user setting up their account after receiving an invitation
    /// </summary>
    /// <param name="requestId">Id og te signin request</param>
    /// <param name="model">Model containing the new password</param>
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