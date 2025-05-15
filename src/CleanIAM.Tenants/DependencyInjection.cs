using CleanIAM.Tenants.Core;
using Marten;

namespace CleanIAM.Tenants;

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
        // Register all aggregates to marten document store
        services.ConfigureMarten(opts =>
        {
            opts.Schema.For<Tenant>().SingleTenanted();
        });
        
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