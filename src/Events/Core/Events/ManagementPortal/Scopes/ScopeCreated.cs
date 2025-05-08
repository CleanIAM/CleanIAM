namespace Events.Core.Events.ManagementPortal.Scopes;

/// <summary>
/// Event triggered when a new scope is created.
/// </summary>
/// <param name="Name">Name of the new scope</param>
/// <param name="DisplayName">Display name of the new scope</param>
/// <param name="Description">Description name of the new scope</param>
/// <param name="Resources">Resources the scope allows access to</param>
public record ScopeCreated(string Name, string DisplayName, string Description, string[] Resources);