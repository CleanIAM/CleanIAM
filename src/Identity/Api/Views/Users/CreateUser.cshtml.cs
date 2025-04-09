using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPortal.Api.Views.Users;

public class CreateUser : PageModel
{
    
    public string? UserName { get; set; }
    
    public void OnGet()
    {
        
    }
}