using Identity.Api.ViewModels.Shared;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("/error")]
public class ErrorController : Controller
{
    [HttpGet]
    public IActionResult Error()
    {
        // If the error originated from the OpenIddict server, render the error details.
        var response = HttpContext.GetOpenIddictServerResponse();
        if (response is not null)
        {
            return View(new ErrorViewModel
            {
                Error = response.Error,
                ErrorDescription = response.ErrorDescription
            });
        }

        return View(new ErrorViewModel());
    }
}
