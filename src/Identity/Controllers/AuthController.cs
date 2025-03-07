using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[Route("/connect")]
public class AuthController : Controller
{
    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        Console.Write("Authorize");
        return Ok();
        //return SignIn(new ClaimsPrincipal());
    }
}