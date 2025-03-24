using OpenIddict.Abstractions;

namespace Identity.Core;

public class SigninRequest
{
    public Guid Id { get; set; }
    public required OpenIddictRequest OidcRequest { get; set; }
}