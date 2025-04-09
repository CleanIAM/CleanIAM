using ManagementPortal.Application.Queries.OpenIdClients;
using ManagementPortal.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace ManagementPortal.Api.Views.Clients;

public class Index(IMessageBus bus) : PageModel
{
    public OpeinIdClient[] Applications { get; set; }

    public async Task OnGetAsync()
    {
        var query = new GetAllOpenIdClientsQuery();
        var applications = await bus.InvokeAsync<IEnumerable<OpeinIdClient>>(query);
        Applications = applications.ToArray();
    }
}