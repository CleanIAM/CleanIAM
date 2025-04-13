using ManagementPortal.Core.OpenIdApplication;

namespace ManagementPortal.Api.Views.Applications;

public class ApplicationCreatedPopup
{
    public Guid Id { get; set; }
    public required string ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public ClientType ClientType { get; set; }
    
}