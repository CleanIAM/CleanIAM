using System.ComponentModel.DataAnnotations;

namespace ManagementPortal.Api.Controllers.Models.Requests.Tenants;

/// <summary>
/// Request to create a new tenant
/// </summary>
public class CreateNewTenantRequest
{
    /// <summary>
    /// Name of the new tenant
    /// </summary>
    [Required]
    public string Name { get; set; }
}