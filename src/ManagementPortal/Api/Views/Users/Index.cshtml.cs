using ManagementPortal.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ManagementPortal.Api.Views.Users;

public class UserPageModel
{
    public required User[] Users { get; set; }
}