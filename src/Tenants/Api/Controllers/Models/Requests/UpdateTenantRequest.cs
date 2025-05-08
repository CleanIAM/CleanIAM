using System.ComponentModel.DataAnnotations;

namespace Tenants.Api.Controllers.Models.Requests;

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