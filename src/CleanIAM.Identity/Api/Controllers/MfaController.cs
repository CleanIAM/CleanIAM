using CleanIAM.Identity.Api.Views.Mfa;
using CleanIAM.Identity.Application.Interfaces;
using CleanIAM.Identity.Application.Queries.Users;
using CleanIAM.Identity.Core.Users;
using Microsoft.AspNetCore.Mvc;
using CleanIAM.SharedKernel.Application.Interfaces.Utils;
using Microsoft.AspNetCore.Authentication;
using Wolverine;

namespace CleanIAM.Identity.Api.Controllers;

/// <summary>
/// Controller for handling MFA (Multi-Factor Authentication) related operations.
/// </summary>
public class MfaController(IMessageBus bus, ISigninRequestService signinRequestService, ITotpValidator totpValidator)
    : Controller
{
    /// <summary>
    /// Shows the MFA input from the user.
    /// </summary>
    [HttpGet("mfa/totp")]
    public async Task<IActionResult> MfaInput()
    {
        if (User.Identity == null || User.Identity.IsAuthenticated == false)
            RedirectToAction("Signin", "Signin");

        var signinRequestResult = await signinRequestService.GetFromClaimsAsync(User);
        if (signinRequestResult.IsError())
            return RedirectToAction("Signin", "Signin");

        return View("Mfa", new MfaViewModel());
    }

    /// <summary>
    /// Handles the MFA input from the user.
    /// </summary>
    [HttpPost("mfa/totp")]
    public async Task<IActionResult> MfaInput([FromForm] MfaViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Mfa", model);

        if (User.Identity is { IsAuthenticated: false })
            return RedirectToAction("Signin", "Signin");

        var signinRequestResult = await signinRequestService.GetFromClaimsAsync(User);
        if (signinRequestResult.IsError() || !signinRequestResult.Value.UserId.HasValue)
            return RedirectToAction("Signin", "Signin");
        var signinRequest = signinRequestResult.Value;


        var query = new GetUserByIdQuery(signinRequest.UserId.Value);
        var user = await bus.InvokeAsync<IdentityUser?>(query);
        if (user == null)
        {
            ModelState.AddModelError("Totp", "User not found");
            return View("Mfa", model);
        }

        if (!user.MfaConfig.IsMfaConfigured)
        {
            ModelState.AddModelError("Totp", "MFA is not configured for this user");
            return View("Mfa", model);
        }

        // Validate the TOTP code
        var res = totpValidator.ValidateTotp(model.Totp, user.MfaConfig.TotpSecretKey);

        if (res.IsError())
        {
            ModelState.AddModelError("Totp", res.ErrorValue.Message);
            return View("Mfa", model);
        }

        // Redirect to authorize endpoint to authorize the client
        var oidcRequestParams = signinRequestService.CreateOidcQueryObject(signinRequest);
        oidcRequestParams["chooseAccount"] = "false"; // Set chooseAccount to false to skip account chooser
        return RedirectToAction("Authorize", "Auth", oidcRequestParams);
    }
    
    /// <summary>
    /// Handles the cancellation of MFA.
    /// </summary>
    [HttpPost("mfa/cancel")]
    public async Task<IActionResult> CancelMfa()
    {
        var signinRequestResult = await signinRequestService.GetFromClaimsAsync(User);
        if (!signinRequestResult.IsError())
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Signin", "Signin", new {request = signinRequestResult.Value.Id});
        }

        var oidcRequestParams = signinRequestService.CreateOidcQueryObject(signinRequestResult.Value);
        oidcRequestParams["error"] = "access_denied";
        return RedirectToAction("Authorize", "Auth", oidcRequestParams);
    }
}