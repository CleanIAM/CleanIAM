namespace Events.Core.Events.Tenants;

/// <summary>
/// Event that is raised when a user is assigned to a tenant
/// </summary>
/// <param name="NewTenantId">Id of the new tenant</param>
/// <param name="TenantName">New tenant name</param>
/// <param name="UserId">Id of the user</param>
public record UserAssignedToTenant(
    Guid NewTenantId,
    string TenantName,
    Guid UserId);