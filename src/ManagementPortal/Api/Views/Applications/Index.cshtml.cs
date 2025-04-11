using ManagementPortal.Application.Queries.OpenIdApplications;
using ManagementPortal.Core.OpenIdApplication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace ManagementPortal.Api.Views.Applications;

public class Index(IMessageBus bus) : PageModel
{
    public OpenIdApplication[] Applications { get; set; }

    public async Task OnGetAsync()
    {
        var query = new GetAllOpenIdApplicationsQuery();
        var applications = await bus.InvokeAsync<IEnumerable<OpenIdApplication>>(query);
        Applications = applications.ToArray();
    }
}