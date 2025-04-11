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

    public IResult OnDelete([FromRoute] Guid id)
    {
        // HTMX request will remove the element from the DOM
        return new NoContentResult();
    }
}