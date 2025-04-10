using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace SharedKernel.Core.Database;

/// <summary>
/// This database context is used only by OpenIdDict, the rest of the application uses document database `Marten`
/// </summary>
public class ApplicationDbContext : DbContext
{
    // Specify the schema name for the database
    public string SchemaName { get;}

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) :
        base(options)
    {
        var dbSchemeName = configuration.GetSection("DbSettings:DatabaseNames")["oidc"];
        Guard.IsNotNullOrEmpty(dbSchemeName, "Db scheme");
        // Get the schema name from the configuration
        SchemaName = dbSchemeName;
    }

    // Add DbSet properties for all OpenIddict entities
    public DbSet<OpenIddictEntityFrameworkCoreApplication<Guid>> OpenIddictEntityFrameworkCoreApplication { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreAuthorization<Guid>> OpenIddictEntityFrameworkCoreAuthorization { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreScope<Guid>> OpenIddictEntityFrameworkCoreScope { get; set; }
    public DbSet<OpenIddictEntityFrameworkCoreToken<Guid>> OpenIddictEntityFrameworkCoreToken { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Specify the default schema for all tables
        modelBuilder.HasDefaultSchema(SchemaName);

        // Configure OpenIdDict objects and make them use Guid as the primary key type
        modelBuilder.UseOpenIddict<Guid>();
        
        base.OnModelCreating(modelBuilder);
    }
}