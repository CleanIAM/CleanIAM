namespace CleanIAM.Tenants.Core.Events;

/// <summary>
/// Event that is raised when a tenant is updated
/// </summary>
/// <param name="Id">If of the updated tenant</param>
/// <param name="Name">New name of the tenant</param>
public record TenantUpdated(Guid Id, string Name);