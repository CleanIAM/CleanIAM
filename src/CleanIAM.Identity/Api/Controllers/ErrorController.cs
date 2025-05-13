using CleanIAM.Identity.Api.ViewModels.Shared;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace CleanIAM.Identity.Api.Controllers;

[Route("/error")]
public class ErrorController : Controller
{
    [HttpGet]
    public IActionResult Error([FromQuery] string error, [FromQuery] string errorDescription)
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

        return View(new ErrorViewModel
        {
            Error = error,
            ErrorDescription = errorDescription
        });
    }
    
    [HttpGet("{errorCode}")]
    public IActionResult Error(int errorCode)
    {
        var error = errorCode switch
        {
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => "Unknown Error"
        };

        return View(new ErrorViewModel
        {
            Error = error,
            ErrorDescription = "An unexpected error occurred."
        });
    }
}