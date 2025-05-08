using Applications;
using DbConfig;
using DotNetEnv;
using SharedKernel;
using SharedKernel.Core.Database;

Env.Load();

// Load object mapping configurations
MapsterConfig.Configure();
Users.MapsterConfig.Configure();
Scopes.MapsterConfig.Configure();
Identity.MapsterConfig.Configure();

var builder = WebApplication.CreateBuilder(args);
string[] assemblies = ["DbSeed"];

builder.Host.AddProjects(assemblies);
// Add databases to allow EF Core to work with the database
builder.Services.AddDatabases(builder.Configuration);
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        // Register the Entity Framework Core stores and models.
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>()
            .ReplaceDefaultEntities<Guid>();
    });


var app = builder.Build();

await app.SeedOpenIddictObjects();
await app.SeedMartenDb();

System.Console.WriteLine("Seeding finished");