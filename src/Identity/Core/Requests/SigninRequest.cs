using Marten.Schema;
using OpenIddict.Abstractions;

namespace Identity.Core.Requests;

/// <summary>
/// Represents a sign-in request.
/// </summary>
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
    /// Indicates whether the user has verified email
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    /// Indicates whether the user is required to go through MFA
    /// </summary>
    public bool IsMfaRequired { get; set; }

    /// <summary>
    /// Indicates whether the user has validated MFA
    /// </summary>
    public bool IsMfaValidated { get; set; }

    /// <summary>
    /// Id of the user
    /// </summary>
    /// <remarks>
    /// This value is filled in after the user has been authenticated.
    /// And user in MFA email validation and other places where we need to know the user.
    /// </remarks>
    public Guid? UserId { get; set; }
}