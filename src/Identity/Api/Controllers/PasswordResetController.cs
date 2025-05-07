using Identity.Api.ViewModels.Shared;
using Identity.Api.Views.PasswordReset;
using Identity.Application.Commands.PasswordReset;
using Identity.Application.Queries.PasswordReset;
using Identity.Core.Events;
using Identity.Core.Requests;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace Identity.Api.Controllers;

/// <summary>
/// Controller for handling password reset flow.
/// </summary>
/// <param name="bus"></param>
[Route("password-reset")]
public class PasswordResetController(IMessageBus bus) : Controller
{
    /// <summary>
    /// Show the form where the user fills in his email, and if the email is valid, a password reset request is sent.
    /// </summary>
    [HttpGet]
    public IActionResult PasswordResetRequest()
    {
        return View();
    }

    /// <summary>
    /// Handle the form submission with email for the password reset request.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PasswordResetRequestAsync([FromForm] PasswordResetRequestViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var command = model.Adapt<SendPasswordResetRequestCommand>();
        var res = await bus.InvokeAsync<Result<PasswordResetRequestSent>>(command);

        if (res.IsError())
        {
            ModelState.AddModelError("Email", res.ErrorValue.Message);
            return View(model);
        }

        return View("PasswordResetRequestSent");
    }

    /// <summary>
    /// Show the form where the user can set a new password.
    /// </summary>
    /// <param name="requestId">Id of the reset password request</param>
    /// <remarks>User is redirected to this page via a link in email</remarks>
    [HttpGet("{requestId:guid}")]
    public async Task<IActionResult> PasswordResetAsync(Guid requestId)
    {
        // Validate if the request exists
        var query = new GetPasswordResetRequestByIdQuery(requestId);
        var request = await bus.InvokeAsync<PasswordResetRequest?>(query);
        if (request is null)
            return View("Error", new ErrorViewModel("Not found", "The password reset request was not found."));

        return View(new PasswordResetViewModel
        {
            RequestId = requestId
        });
    }

    /// <summary>
    /// Handle the form submission with new password for the password reset request.
    /// </summary>
    [HttpPost("{requestId:guid}")]
    public async Task<IActionResult> PasswordResetAsync([FromForm] PasswordResetViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var command = model.Adapt<ResetPasswordCommand>();
        var res = await bus.InvokeAsync<Result<PasswordReset>>(command);
        if (res.IsError())
        {
            ModelState.AddModelError("ConfirmPassword", res.ErrorValue.Message);
            return View(model);
        }

        return View("PasswordResetSuccess");
    }
}