using CleanIAM.UrlShortener.Api.Views.Shared;
using CleanIAM.UrlShortener.Application.Queries;
using CleanIAM.UrlShortener.Core;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace CleanIAM.UrlShortener.Api.Controllers;

/// <summary>
/// The main controller for the URL shortener.
/// </summary>
/// <param name="bus"></param>
public class UrlShortenerController(IMessageBus bus) : Controller
{
    /// <summary>
    /// The endpoint handling the shortened url access
    /// </summary>
    [HttpGet("/url-shortener/{id:guid}")]
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