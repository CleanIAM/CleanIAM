using ManagementPortal.Core.OpenIdApplication;

namespace ManagementPortal.Core.Events.OpenIdApplications;

public record OpenIdApplicationUpdated(
    Guid Id,
    ApplicationType? ApplicationType,
    string ClientId,
    ClientType? ClientType,
    ConsentType? ConsentType,
    string? DisplayName,
    HashSet<string> Scopes,
    HashSet<Uri> PostLogoutRedirectUris,
    HashSet<Uri> RedirectUris);