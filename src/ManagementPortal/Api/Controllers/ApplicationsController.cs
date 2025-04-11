using ManagementPortal.Api.Views.Applications.Edit;
using Microsoft.AspNetCore.Mvc;

namespace ManagementPortal.Api.Controllers;

[Route("api/applications")]
public class ApplicationsController: Controller
{
    
    [HttpGet]
    public IActionResult Index(Guid id)
    {
        return View();
    }
    
    [HttpGet("{id:guid}/edit")]
    public IActionResult Edit(Guid id)
    {
        return View();
    }

    /// <summary>
    /// HTMX endpoint to remove a URI Item from the list.
    /// </summary>
    [HttpDelete("/{id:guid}/edit/uri")]
    public IActionResult RemoveUri()
    {
        // HTMX request will remove the element from the DOM
        return new OkResult();
    }

    /// <summary>
    /// HTMX endpoint to add a new URI Item to the list.
    /// </summary>
    /// <param name="id">application id</param>
    /// <param name="uri">uri to include into generated html</param>
    /// <returns></returns>
    [HttpPost("/{id:guid}/edit/uri")]
    public IActionResult AddUri([FromRoute] Guid id, [FromForm(Name = "new-redirect-uri")] string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return BadRequest("URI cannot be empty");
        }

        if (!Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
        {
            return BadRequest("Invalid URI format.");
        }

        // Return the partial view for HTMX to add to the DOM
        return View("Edit/UriItem", new UriItemModel { Uri = parsedUri, Id = id });
    }
}