using Microsoft.AspNetCore.Mvc;
using UrlShortner.Api.Views.Shared;
using UrlShortner.Application.Queries;
using UrlShortner.Core;
using Wolverine;

namespace UrlShortner.Api.Controllers;

/// <summary>
/// The main controller for the URL shortener.
/// </summary>
/// <param name="bus"></param>
public class UrlShortnerController(IMessageBus bus) : Controller
{
    /// <summary>
    /// The endpoint handling the shortened url access
    /// </summary>
    [HttpGet("/url-shortner/{id:guid}")]
    public async Task<IActionResult> GetUrl(Guid id)
    {
        var query = new GetShortenedUrlByIdQuery(id);
        var url = await bus.InvokeAsync<ShortenedUrl?>(query);
        
        if (url == null)
            return View("Error",
                new ErrorViewModel { Error = "Wrong url", ErrorDescription = "This url goes nowhere" });
        
        return Redirect(url.OriginalUrl);
    }
}