namespace CleanIAM.Applications.Core.Events;

public record OpenIdApplicationCreated(
    Guid Id,
    ApplicationType? ApplicationType,
    string ClientId,
    string? ClientSecret,
    ClientType? ClientType,
    ConsentType? ConsentType,
    string? DisplayName,
    HashSet<string> Scopes,
    HashSet<Uri> PostLogoutRedirectUris,
    HashSet<Uri> RedirectUris);