using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Core.Database;

/// <summary>
/// This database context is used only by OpenIddict, the rest of the application uses document database `Marten`
/// </summary>
public class ApplicationDbContext : DbContext
{
    // Specify the schema name for the database

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) :
        base(options)
    {
    }
}