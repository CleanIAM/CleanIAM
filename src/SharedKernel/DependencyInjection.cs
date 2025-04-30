using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using JasperFx.CodeGeneration;
using Mapster;
using Marten;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedKernel.Application.Interfaces;
using SharedKernel.Application.Swagger;
using SharedKernel.Core.Database;
using SharedKernel.Infrastructure.Services;
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
        var dbConnectionString = configuration.GetSection("DbSettings:ConnectionStrings")["oidc"];
        Guard.IsNotNullOrEmpty(dbConnectionString, "Connection string not provided");

        serviceCollection.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(dbConnectionString); });

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

    public static IServiceCollection AddSwagger(this IServiceCollection services, string title,
        string[] assemblies)
    {
        services.AddSwaggerGen(options =>
        {
            // Configure basic swagger info 
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = title,
                Version = "v1",
                Description = "CleanIAM API",
            });

            // Create swagger auth definition
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter your token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            // Apply auth swagger definition
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Add xml comments from all assemblies to swagger
            foreach (var assembly in assemblies)
            {
                var assemblyXmlPath = Path.Combine(AppContext.BaseDirectory, $"{assembly}.xml");
                options.IncludeXmlComments(assemblyXmlPath);
            }

            // Make all strings nullable by defaults
            options.SupportNonNullableReferenceTypes();

            options.SchemaFilter<MakeAllPropertiesRequiredFilter>();
        });

        return services;
    }


    public static IServiceCollection AddUtils(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMapster();

        services.AddTransient<IAppConfiguration>(_ => ParseConfiguration(configuration));

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });
        return services;
    }


    private static IAppConfiguration ParseConfiguration(IConfiguration configuration)
    {
        var identityBaseUrl = configuration.GetSection("Identity")["BaseUrl"];
        var useUrlShortener = configuration.GetSection("UrlShortener:UseUrlShortener").Get<bool>();
        var urlShortenerBaseUrl = configuration.GetSection("UrlShortener")["BaseUrl"];

        Guard.IsNotNullOrEmpty(identityBaseUrl, "Identity base url not provided");
        Guard.IsNotNull(useUrlShortener, "Use url shortener not provided");
        if (useUrlShortener)
            Guard.IsNotNullOrEmpty(urlShortenerBaseUrl, "Url shortener base url not provided");


        var appConfiguration = new AppConfiguration
        {
            IdentityBaseUrl = identityBaseUrl,
            UseUrlShortener = useUrlShortener,
            UrlShortenerBaseUrl = urlShortenerBaseUrl
        };

        return appConfiguration;
    }
}