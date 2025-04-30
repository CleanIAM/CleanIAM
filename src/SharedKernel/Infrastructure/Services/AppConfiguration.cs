using SharedKernel.Application.Interfaces;

namespace SharedKernel.Infrastructure.Services;

public class AppConfiguration : IAppConfiguration
{
    public required string IdentityBaseUrl { get; init; }
    public bool UseUrlShortener { get; init; }
    public string? UrlShortenerBaseUrl { get; init; }
}