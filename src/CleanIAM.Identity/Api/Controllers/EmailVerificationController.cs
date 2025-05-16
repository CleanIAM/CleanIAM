using CleanIAM.Identity.Api.ViewModels.Shared;
using CleanIAM.Identity.Api.Views.EmailVerification;
using CleanIAM.Identity.Application.Commands.EmailVerification;
using CleanIAM.Identity.Application.Interfaces;
using CleanIAM.Identity.Core.Events;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication;
using Wolverine;

namespace CleanIAM.Identity.Api.Controllers;

/// <summary>
/// Controller handling email verification flow
/// </summary>
[Route("/email-verification")]
public class EmailVerificationController(ISigninRequestService signinRequestService, IMessageBus bus) : Controller
{
    /// <summary>
    /// Show a page announcing that email verification is required
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> VerifyEmail()
    {
        // Check if the user is authenticated and is in the correct step of the signin flow
        var signinRequest = await signinRequestService.GetFromClaimsAsync(User);
        if (signinRequest.IsError() || signinRequest.Value.UserId is null)
            return View("Error", new ErrorViewModel { Error = "Error", ErrorDescription = "Invalid signin request" });

        // If the user is already verified, redirect to the signin page
        if(signinRequest.Value.IsEmailVerified)
            return RedirectToAction("Signin", "Signin", new{ request = signinRequest.Value.Id });
        
        // Show the view with an email verification form
        return View(new VerifyEmailViewModel());
    }

    /// <summary>
    /// Handle the user requesting email for email verification
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SendEmailVerification([FromForm] VerifyEmailViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View("VerifyEmail", model);

        // Check if the user is authenticated and is in the correct step of the signin flow
        var signinRequestResult = await signinRequestService.GetFromClaimsAsync(User);
        if (signinRequestResult.IsError() || signinRequestResult.Value.UserId is null)
            return View("Error", new ErrorViewModel { Error = "Error", ErrorDescription = "Invalid signin request" });
        var signinRequest = signinRequestResult.Value;

        // Send email verification
        var command = new SendEmailVerificationRequestCommand((Guid)signinRequest.UserId);
        var res = await bus.InvokeAsync<Result<EmailVerificationRequestSent>>(command, cancellationToken);

        // If the request was not sent, show an error
        if (res.IsError())
        {
            ModelState.AddModelError("requestId", res.ErrorValue.Message);
            return View("VerifyEmail", model);
        }

        // If the request was sent, show a success message
        return View("VerificationEmailSent", model.Adapt<VerificationEmailSentViewModel>());
    }

    /// <summary>
    /// Handle the user clicking on the email verification link
    /// </summary>
    /// <param name="id">Id of the verification request</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> EmailVerified([FromRoute] Guid id)
    {
        var command = new VerifyEmailCommand(id);
        var res = await bus.InvokeAsync<Result<UserEmailVerified>>(command);

        if (res.IsError())
            return View("Error", new ErrorViewModel
            {
                Error = "Email verification failed",
                ErrorDescription = res.ErrorValue.Message
            });

        await HttpContext.SignOutAsync();

        return View();
    }
}