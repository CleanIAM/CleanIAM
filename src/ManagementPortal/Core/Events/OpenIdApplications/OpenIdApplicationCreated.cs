using ManagementPortal.Core.OpenIdApplication;

namespace ManagementPortal.Core.Events.OpenIdApplications;

public record OpenIdApplicationCreated(
    Guid Id,
    ApplicationType? ApplicationType,
    string ClientId,
    string? ClientSecret,
    ClientType? ClientType,
    ConsentType? ConsentType,
    string? DisplayName,
    HashSet<string> Permissions,
    HashSet<Uri> PostLogoutRedirectUris,
    HashSet<Uri> RedirectUris,
    HashSet<string> Requirements,
    Dictionary<string, string> Settings);