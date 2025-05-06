using Marten.Schema;
using OpenIddict.Abstractions;

namespace Identity.Core.Requests;

[SingleTenanted]
public class SigninRequest
{
    /// <summary>
    /// Id of the request
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Oidc signin request values
    /// </summary>
    public required OpenIddictRequest OidcRequest { get; set; }

    /// <summary>
    /// Id of the user
    /// </summary>
    /// <remarks>
    /// This value is filled in after the user has been authenticated.
    /// And user in MFA email validation and other places where we need to know the user.
    /// </remarks>
    public Guid? UserId { get; set; }
}