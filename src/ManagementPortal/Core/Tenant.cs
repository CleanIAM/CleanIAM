using Marten.Schema;

namespace ManagementPortal.Core;

/// <summary>
/// Represents a tenant in the system.
/// </summary>
[SingleTenanted]
public class Tenant
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}