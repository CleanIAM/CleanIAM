namespace ManagementPortal.Api.Controllers.Models;

/// <summary>
/// Represents a tenant in the system.
/// </summary>
public class ApiTenantModel
{
    /// <summary>
    /// The unique identifier for the tenant.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the tenant.
    /// </summary>
    public required string Name { get; set; }
}