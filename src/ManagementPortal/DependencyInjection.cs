using CommunityToolkit.Diagnostics;
using SharedKernel.Core.Database;

namespace ManagementPortal;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenIddict(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            });
        return serviceCollection;
    }
}