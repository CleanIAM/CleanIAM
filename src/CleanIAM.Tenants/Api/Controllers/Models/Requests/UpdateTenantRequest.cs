using System.ComponentModel.DataAnnotations;

namespace CleanIAM.Tenants.Api.Controllers.Models.Requests;

/// <summary>
/// Request to update a tenant
/// </summary>
public class UpdateTenantRequest
{
    /// <summary>
    /// New name for the tenant
    /// </summary>
    [Required]
    [Length(3, 32, ErrorMessage = "Name length must be between 3 and 32 character long")]
    public required string Name { get; set; }
}