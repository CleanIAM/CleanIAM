namespace CleanIAM.Identity.Api.ViewModels.Shared;

public class ErrorViewModel(string? error = null, string? errorDescription = null)
{
    public string? Error { get; set; } = error;
    public string? ErrorDescription { get; set; } = errorDescription;
}