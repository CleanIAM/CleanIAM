using ManagementPortal.Api.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPortal.Api.Views.MainPage;

public class MainPageModel : PageModel
{
    public long UserCount { get; set; }
    public long ApplicationsCount { get; set; }
    public int AuthRequestCount { get; set; } = 142; // Sample data
    public int SuccessfulLogins { get; set; } = 125; // Sample data
    public int FailedLogins { get; set; } = 17; // Sample data
    public double ServerUptime { get; set; } = 99.9; // Sample data
    public int CpuUsage { get; set; } = 23; // Sample data
    public int MemoryUsage { get; set; } = 68; // Sample data

    public required List<ActivityItem> RecentActivity { get; set; }
    


}