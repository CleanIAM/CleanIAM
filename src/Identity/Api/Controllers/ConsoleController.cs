using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[Route("/console")]
public class ConsoleController: Controller
{
    
    
    [HttpGet("home")]
    public IActionResult Home()
    {
        return View();
    }
}