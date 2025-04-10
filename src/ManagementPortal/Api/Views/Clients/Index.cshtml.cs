using ManagementPortal.Application.Queries.OpenIdClients;
using ManagementPortal.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace ManagementPortal.Api.Views.Clients;

public class Index(IMessageBus bus) : PageModel
{
    public OpenIdApplication[] Applications { get; set; }

    public async Task OnGetAsync()
    {
        var query = new GetAllOpenIdClientsQuery();
        var applications = await bus.InvokeAsync<IEnumerable<OpenIdApplication>>(query);
        Applications = applications.ToArray();
    }
}