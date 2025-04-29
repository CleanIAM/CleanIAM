using Microsoft.AspNetCore.Mvc;
using UrlShortner.Api.Views.Shared;
using UrlShortner.Application.Queries;
using UrlShortner.Core;
using Wolverine;

namespace UrlShortner.Api.Controllers;

public class UrlShortnerController(IMessageBus bus): Controller
{
    
    [HttpGet("/url-shortner/{id:guid}")]
    public async Task<IActionResult> GetUrl(Guid id)
    {
        var query = new GetShortenedUrlByIdQuery(id);
        var url = await bus.InvokeAsync<ShortenedUrl?>(query);
        if(url == null)
            return View("Error", new ErrorViewModel{Error = "Wrong url", ErrorDescription = "This url goes nowhere" });
        return Redirect(url.OriginalUrl);
    }
    
}