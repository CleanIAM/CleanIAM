using System.Reflection;
using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using JasperFx.CodeGeneration;
using Marten;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Core.Database;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Marten;

namespace SharedKernel;

public static class DependencyInjection
{

    public static IHostBuilder UseProjects(this IHostBuilder host, string[] assemblies)
    {
        host.UseWolverine(opts =>
        {
            foreach (var assembly in assemblies)
                opts.Discovery.IncludeAssembly(Assembly.Load(assembly));
            opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;

            opts.Policies.AutoApplyTransactions();
            opts.Policies.UseDurableLocalQueues();
            opts.UseFluentValidation();
            opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;
        });
        
        return host;
    }
    
    public static IServiceCollection AddDatabases(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {

        serviceCollection.AddDbContext<ApplicationDbContext>(options =>
        {
            var dbConnectionString = configuration.GetSection("DbSettings:ConnectionStrings")["oidc"];
            Guard.IsNotNullOrEmpty(dbConnectionString, "Connection string not provided");
            options.UseNpgsql(dbConnectionString);
            options.UseOpenIddict();
        });

        serviceCollection.AddMarten(configuration);

        return serviceCollection;
    }
    
    public static IServiceCollection AddMarten(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSchemeName = configuration.GetSection("DbSettings:DatabaseNames")["MartenDb"];
        var connectionString =
            configuration.GetSection("DbSettings:ConnectionStrings")["MartenDb"];
        Guard.IsNotNullOrEmpty(dbSchemeName, "Db scheme");
        Guard.IsNotNullOrEmpty(connectionString, "Connection string");

        services.AddMarten(opts =>
            {
                opts.Connection(connectionString);
                opts.DatabaseSchemaName = dbSchemeName;
                opts.Policies.AllDocumentsAreMultiTenanted();
            })
            .ApplyAllDatabaseChangesOnStartup()
            .UseLightweightSessions()
            .IntegrateWithWolverine();

        // TODO: Add multitenancy support
        // services.AddMartenTenancyDetection(opts =>
        // {
        //     // Tenant name is organization id
        //     opts.IsClaimTypeNamed(SharedKernelConstants.OrganizationId);
        //     opts.DefaultIs("default_tenant");
        // });

        return services;
    }


    public static IServiceCollection AddUtils(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        return services;
    }


}