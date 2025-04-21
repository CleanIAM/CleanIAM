using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

[Route("/")]
public class MainPageController(IMessageBus bus): Controller
{
    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> IndexAsync()
    {
        return View();
    }
    
    
    /// <summary>
    /// HTMX helper method to add a remove element from the page.
    /// </summary>
    [HttpDelete("htmx-element")]
    public IActionResult RemoveHtmxElement()
    {
        return Ok();
    }
}