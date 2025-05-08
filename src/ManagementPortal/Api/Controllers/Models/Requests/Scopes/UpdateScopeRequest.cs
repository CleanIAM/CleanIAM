namespace ManagementPortal.Api.Controllers.Models.Requests.Scopes;

/// <summary>
/// Request to update a scope.
/// </summary>
public class UpdateScopeRequest
{
    /// <summary>
    /// The display name of the scope.
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// The description of the scope.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The resources the scopes allows access to.
    /// </summary>
    public required string[] Resources { get; set; }
}