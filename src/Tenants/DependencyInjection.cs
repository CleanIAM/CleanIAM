namespace Tenants;

public static class DependencyInjection
{
    /// <summary>
    /// Register configuration specific for the tenants project.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddTenants(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure custom mapster config

        return services;
    }

    /// <summary>
    /// Register runtime configuration specific for the tenants project.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseTenants(this WebApplication app)
    {
        return app;
    }
}