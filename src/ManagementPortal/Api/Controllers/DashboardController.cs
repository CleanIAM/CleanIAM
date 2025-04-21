using ManagementPortal.Api.Views.Dashboard;
using ManagementPortal.Application.Queries.OpenIdApplications;
using ManagementPortal.Application.Queries.Users;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

[Route("/dashboard")]
public class DashboardController(IMessageBus bus): Controller
{
    [HttpGet("")]
    public async Task<IActionResult> IndexAsync()
    {
        // Get user count
        var userQuery = new GetAllUsersCountQuery();
        var usersCount = await bus.InvokeAsync<long>(userQuery);
       

        // Get applications count
        var applicationsQuery = new GetAllOpenIdApplicationCountQuery();
        var applicationsCount = await bus.InvokeAsync<long>(applicationsQuery);

        // Initialize sample activity items
        List<ActivityItem> recentActivity =
        [
            new()
            {
                Type = "User",
                Details = "john.doe@example.com",
                TimeAgo = "10 minutes ago"
            },

            new()
            {
                Type = "Applications",
                Action = "New applications created",
                Details = "My Web App",
                TimeAgo = "1 hour ago"
            },

            new()
            {
                Type = "Token",
                Action = "Token issued",
                Details = "Mobile Application",
                TimeAgo = "3 hours ago"
            }
        ];
        
        return View(new MainPageModel
        {
            UserCount = usersCount,
            ApplicationsCount = applicationsCount,
            RecentActivity = recentActivity
        });
    }
    
    
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }).ToArray());
    }
    
}

public class ActivityItem
{
    public string Type { get; set; }
    public string Action { get; set; }
    public string Details { get; set; }
    public string TimeAgo { get; set; }
}