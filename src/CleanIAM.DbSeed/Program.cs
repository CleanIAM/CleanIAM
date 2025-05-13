using CleanIAM.Applications;
using DbConfig;
using DotNetEnv;
using CleanIAM.SharedKernel;
using CleanIAM.SharedKernel.Core.Database;

Env.Load();

// Load object mapping configurations
MapsterConfig.Configure();
CleanIAM.Users.MapsterConfig.Configure();
CleanIAM.Scopes.MapsterConfig.Configure();
CleanIAM.Identity.MapsterConfig.Configure();

var builder = WebApplication.CreateBuilder(args);
string[] assemblies = ["CleanIAM.DbSeed"];

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