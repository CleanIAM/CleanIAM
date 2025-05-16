using CleanIAM.UrlShortener.Core;
using Marten;

namespace CleanIAM.UrlShortener;

public static class DependencyInjection
{
    /// <summary>
    /// Register configuration specific for the url shortener project.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddUrlShortener(this IServiceCollection services, IConfiguration configuration)
    {
        
        // Register all aggregates to marten document store
        services.ConfigureMarten(opts =>
        {
            opts.Schema.For<ShortenedUrl>().SingleTenanted();
        });

        return services;
    }

    /// <summary>
    /// Register runtime configuration specific for the url shortener project.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseUrlShortener(this WebApplication app)
    {
        return app;
    }
}