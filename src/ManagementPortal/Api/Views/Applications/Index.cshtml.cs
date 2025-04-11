using ManagementPortal.Core.OpenIdApplication;

namespace ManagementPortal.Api.Views.Applications;

public class ApplicationPageModel
{
    public required OpenIdApplication[] Applications { get; set; }
    
}