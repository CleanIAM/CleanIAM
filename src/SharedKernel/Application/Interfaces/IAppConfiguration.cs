namespace SharedKernel.Application.Interfaces;

public interface IAppConfiguration
{
    /// <summary>
    /// base url for the identity service
    /// </summary>
    /// <remarks>Without the trailing slash</remarks>
    public string IdentityBaseUrl { get; init; }
    
    /// <summary>
    /// base url for the management portal website
    /// </summary>
    /// <remarks>Without the trailing slash</remarks>
    public string ManagementPortalBaseUrl { get; init; }
    
    /// <summary>
    /// Indicates whether the URL shortener feature is enabled.
    /// </summary>
    /// <remarks>If set to true, "UrlShortenerBaseUrl" must be also provided</remarks>
    public bool UseUrlShortener { get; init; }

    /// <summary>
    /// Base URL for the URL shortener service
    /// </summary>
    /// <remarks>Without the trailing slash</remarks>
    public string? UrlShortenerBaseUrl { get; init; }
}