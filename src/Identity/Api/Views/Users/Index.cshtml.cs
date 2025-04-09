using ManagementPortal.Application.Queries;
using ManagementPortal.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace ManagementPortal.Api.Views.Users;

public class Index(IMessageBus bus) : PageModel
{
    
    public required List<User> UserList { get; set; }
    
    public async Task OnGetAsync()
    {
        var query = new GetAllUsersQuery();
        var users = await bus.InvokeAsync<IEnumerable<User>>(query);
        
        UserList = users.ToList();   
    }
}