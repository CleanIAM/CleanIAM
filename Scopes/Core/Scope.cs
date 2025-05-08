namespace Scopes.Core;

/// <summary>
/// Represents a scope in the OpenID Connect system.
/// </summary>
/// <remarks>
/// This class is only used for abstraction, the real scope object stored in database is
/// OpenIddictEntityFrameworkCoreScope&lt;Guid&gt; managed by OpenIddict.
/// </remarks>
public class Scope
{
    /// <summary>
    /// The name of the scope.
    /// </summary>
    public required string Name { get; set; }

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