using Microsoft.Extensions.Primitives;

namespace Identity.Api.Views.Auth;

public class AuthorizeViewModel
{
    /// <summary>
    /// Object representing the OIDC request properties
    /// </summary>
    public Dictionary<string, StringValues> OidcRequest { get; set; }

    /// <summary>
    /// The name of the authenticated user
    /// </summary>
    public required string Name { get; set; }
}