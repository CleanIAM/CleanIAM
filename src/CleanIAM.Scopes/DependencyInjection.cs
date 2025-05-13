namespace CleanIAM.Scopes;

public static class DependencyInjection
{
    /// <summary>
    /// Register configuration specific for the scopes project.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddScopes(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure custom mapster config
        MapsterConfig.Configure();

        return services;
    }

    /// <summary>
    /// Register runtime configuration specific for the scopes project.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseScopes(this WebApplication app)
    {
        return app;
    }
}