using Marten.Schema;

namespace ManagementPortal.Core;

/// <summary>
/// Represents a tenant in the system.
/// </summary>
[SingleTenanted]
public class Tenant
{
    /// <summary>
    /// Unique identifier for the tenant.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the tenant.
    /// </summary>
    public required string Name { get; set; }
}