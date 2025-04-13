using ManagementPortal.Core.OpenIdApplication;

namespace ManagementPortal.Core.Events;

public record OpenIdApplicationUpdated(
    Guid Id,
    ApplicationType? ApplicationType,
    string ClientId,
    ClientType? ClientType,
    ConsentType? ConsentType,
    string? DisplayName,
    HashSet<string> Permissions,
    HashSet<Uri> PostLogoutRedirectUris,
    HashSet<Uri> RedirectUris,
    HashSet<string> Requirements,
    Dictionary<string, string> Settings);