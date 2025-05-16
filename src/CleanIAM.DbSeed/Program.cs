using System.Text;
using CleanIAM.Applications;
using DbConfig;
using DotNetEnv;
using CleanIAM.SharedKernel;
using CleanIAM.SharedKernel.Core.Database;
using CommunityToolkit.Diagnostics;
using Microsoft.IdentityModel.Tokens;

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

var encryptionKey = builder.Configuration.GetSection("OpenIddict")["EncryptionKey"];
Guard.IsNotNullOrEmpty(encryptionKey, "Encryption key");

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        // Register the Entity Framework Core stores and models.
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>()
            .ReplaceDefaultEntities<Guid>();
    }).AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token")
            .SetIntrospectionEndpointUris("/connect/introspect")
            .SetEndSessionEndpointUris("/connect/endsession")
            .SetUserInfoEndpointUris("/connect/userinfo");

        options.AllowAuthorizationCodeFlow(); // For FE clients
        options.AllowClientCredentialsFlow() // For BE clients
            .RequireProofKeyForCodeExchange();
        options.AllowRefreshTokenFlow();

        options.AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableEndSessionEndpointPassthrough()
            .EnableUserInfoEndpointPassthrough()
            .EnableStatusCodePagesIntegration();

        options.DisableAccessTokenEncryption();

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(encryptionKey)));
    });


var app = builder.Build();

await app.SeedOpenIddictObjects();
await app.SeedMartenDb();

await app.SeedTesting();

System.Console.WriteLine("Seeding finished");