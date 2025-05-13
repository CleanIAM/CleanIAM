using System.ComponentModel.DataAnnotations;

namespace CleanIAM.Tenants.Api.Controllers.Models.Requests;

/// <summary>
/// Request to create a new tenant
/// </summary>
public class CreateNewTenantRequest
{
    /// <summary>
    /// Name of the new tenant
    /// </summary>
    [Required]
    [Length(3, 32, ErrorMessage = "Name length must be between 3 and 32 character long")]
    public string Name { get; set; }
}