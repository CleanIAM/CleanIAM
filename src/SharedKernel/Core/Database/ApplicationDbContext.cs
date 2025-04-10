using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Specify the default schema for all tables
        modelBuilder.HasDefaultSchema(SchemaName);

        // Configure OpenIdDict objects and make them use Guid as the primary key type
        modelBuilder.UseOpenIddict<Guid>();
        
        base.OnModelCreating(modelBuilder);
    }
}