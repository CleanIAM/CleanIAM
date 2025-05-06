namespace Events.Core.Events.ManagementPortal.Tenants;

/// <summary>
/// Event that is raised when a user is assigned to a tenant
/// </summary>
/// <param name="NewTenantId">Id of the new tenant</param>
/// <param name="OldTenantId">Id of the old tenant</param>
/// <param name="TenantName">New tenant name</param>
/// <param name="UserId">Id of the user</param>
/// <param name="UserFirstName">First name of the user</param>
/// <param name="UserLastName">Last name of the user</param>
public record UserAssignedToTenant(
    Guid NewTenantId,
    Guid OldTenantId,
    string TenantName,
    Guid UserId,
    string UserFirstName,
    string UserLastName);