namespace ManagementPortal.Core.Events.Tenants;

/// <summary>
/// Event that is raised when a user is assigned to a tenant
/// </summary>
/// <param name="TenantId">Id of the tenant</param>
/// <param name="TenantName">New tenant name</param>
/// <param name="UserId">Id of the user</param>
/// <param name="UserFirstName">First name of the user</param>
/// <param name="UserLastName">Last name of the user</param>
public record UserAssignedToTenant(
    Guid TenantId,
    string TenantName,
    Guid UserId,
    string UserFirstName,
    string UserLastName);