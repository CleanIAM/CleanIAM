using ManagementPortal.Application.Queries.Users;
using ManagementPortal.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace ManagementPortal.Api.Views.Users;

public class Index(IMessageBus bus) : PageModel
{
    public User[] Users { get; set; }

    public async Task OnGetAsync()
    {
        var query = new GetAllUsersQuery();
        var users = await bus.InvokeAsync<IEnumerable<User>>(query);

        Users = users.ToArray();
    }
}