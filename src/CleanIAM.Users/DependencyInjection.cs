using CleanIAM.Users.Core;
using Marten;

namespace CleanIAM.Users;

public static class DependencyInjection
{
    /// <summary>
    /// Register configuration specific for the users project.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddUsers(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure custom mapster config
        MapsterConfig.Configure();
        
        // Register all aggregates to marten document store
        services.ConfigureMarten(opts =>
        {
            opts.Schema.For<User>();
        });

        return services;
    }

    /// <summary>
    /// Register runtime configuration specific for the users project.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseUsers(this WebApplication app)
    {
        return app;
    }
}