namespace CleanIAM.Events.Core.Events.Tenants;

/// <summary>
/// Event that is raised when a tenant is updated
/// </summary>
/// <param name="Id">Tenant id</param>
/// <param name="Name">New tenant name</param>
public record TenantUpdated(Guid Id, string Name);