using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPortal.Api.Views.Applications.Edit;

public class UriEditModel : PageModel
{
    public Uri? Uri { get; set; }

    public Guid Id { get; set; }

    public void OnGet([FromRoute] Guid id, [FromQuery] Uri uri)
    {
        Uri = uri;
        Id = id;
    }
    
    public IActionResult OnPost([FromRoute] Guid id, [FromBody] string uri)
    {

        if (!Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri))
        {
            return BadRequest("Invalid URI format.");
        }
        
        Console.WriteLine($"Uri: {uri}");
        Uri = parsedUri;
        Id = id;

        return Page();
    }

    public IActionResult OnDelete([FromRoute] Guid id)
    {
        // HTMX request will remove the element from the DOM
        return new NoContentResult();
    }
}