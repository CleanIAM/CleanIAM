using CleanIAM.SharedKernel.Application.Interfaces;

namespace CleanIAM.SharedKernel.Infrastructure.Services;

public class AppConfiguration : IAppConfiguration
{
    public required string IdentityBaseUrl { get; init; }
    public required string ManagementPortalBaseUrl { get; init; }
    public bool UseUrlShortener { get; init; }
    public string? UrlShortenerBaseUrl { get; init; }
}