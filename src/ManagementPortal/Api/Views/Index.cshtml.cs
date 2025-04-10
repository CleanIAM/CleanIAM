using ManagementPortal.Application.Queries.Users;
using ManagementPortal.Application.Queries.OpenIdClients;
using ManagementPortal.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace ManagementPortal.Api.Views;

public class Index : PageModel
{
    private readonly IMessageBus _bus;
    
    public int UserCount { get; private set; }
    public int ClientCount { get; private set; }
    public int AuthRequestCount { get; private set; } = 142; // Sample data
    public int SuccessfulLogins { get; private set; } = 125; // Sample data
    public int FailedLogins { get; private set; } = 17; // Sample data
    public double ServerUptime { get; private set; } = 99.9; // Sample data
    public int CpuUsage { get; private set; } = 23; // Sample data
    public int MemoryUsage { get; private set; } = 68; // Sample data
    
    // Dashboard activity feed items (would be fetched from an audit log in a real system)
    public List<ActivityItem> RecentActivity { get; private set; } = new List<ActivityItem>();
    
    public Index(IMessageBus bus)
    {
        _bus = bus;
    }
    
    public async Task OnGetAsync()
    {
        // Get user count
        var userQuery = new GetAllUsersQuery();
        var users = await _bus.InvokeAsync<IEnumerable<User>>(userQuery);
        UserCount = users.Count();
        
        // Get client count
        var clientQuery = new GetAllOpenIdClientsQuery();
        var clients = await _bus.InvokeAsync<IEnumerable<OpenIdApplication>>(clientQuery);
        ClientCount = clients.Count();
        
        // Initialize sample activity items
        RecentActivity = new List<ActivityItem>
        {
            new ActivityItem
            {
                Type = "User",
                Action = "New user registered",
                Details = "john.doe@example.com",
                TimeAgo = "10 minutes ago"
            },
            new ActivityItem
            {
                Type = "Client",
                Action = "New client created",
                Details = "My Web App",
                TimeAgo = "1 hour ago"
            },
            new ActivityItem
            {
                Type = "Token",
                Action = "Token issued",
                Details = "Mobile Application",
                TimeAgo = "3 hours ago"
            }
        };
    }
    
    public class ActivityItem
    {
        public string Type { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public string TimeAgo { get; set; }
    }
}