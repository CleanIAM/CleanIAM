namespace ManagementPortal.Core.Events.Scopes;

/// <summary>
/// Event triggered when a scope is updated.
/// </summary>
/// <param name="Name">Name of the new scope</param>
/// <param name="DisplayName">Display name of the new scope</param>
/// <param name="Description">Description name of the new scope</param>
/// <param name="Resources">Resources the scope allows access to</param>
public record ScopeUpdated(string Name, string DisplayName, string Description, string[] Resources);