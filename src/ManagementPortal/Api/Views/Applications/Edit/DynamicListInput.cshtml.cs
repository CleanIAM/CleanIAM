namespace ManagementPortal.Api.Views.Applications.Edit;

public class DynamicListInput
{
    public string InputName { get; set; } = string.Empty;
    
    public string Placeholder { get; set; } = string.Empty;
    
    public string? Value { get; set; } = string.Empty;
    
    public string? HxPostUrl { get; set; } = string.Empty;
    
    public string HxTarget { get; set; } = string.Empty;
    
}