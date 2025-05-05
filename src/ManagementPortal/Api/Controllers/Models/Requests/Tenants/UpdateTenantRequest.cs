using System.ComponentModel.DataAnnotations;

namespace ManagementPortal.Api.Controllers.Models.Requests.Tenants;

/// <summary>
/// Request to update a tenant
/// </summary>
public class UpdateTenantRequest
{
    /// <summary>
    /// New name for the tenant
    /// </summary>
    [Required]
    public required string Name { get; set; }
}