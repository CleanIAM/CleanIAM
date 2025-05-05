namespace ManagementPortal.Core.Events.Tenants;

/// <summary>
/// Event that is raised when a new tenant is created
/// </summary>
/// <param name="Id">Id of the new tenant</param>
/// <param name="Name">Name of the new tenant</param>
public record NewTenantCreated(Guid Id, string Name);