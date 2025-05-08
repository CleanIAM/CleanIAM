using System.ComponentModel.DataAnnotations;

namespace Tenants.Api.Controllers.Models.Requests;

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