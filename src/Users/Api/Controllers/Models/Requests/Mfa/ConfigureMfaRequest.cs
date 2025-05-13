using System.ComponentModel.DataAnnotations;

namespace Users.Api.Controllers.Models.Requests.Mfa;

/// <summary>
/// Request model for configuring MFA
/// </summary>
public class ConfigureMfaRequest
{
    /// <summary>
    /// Totp code to verify the user has successfully connected to some authenticator app
    /// </summary>
    [Required]
    [Length(6, 6, ErrorMessage = "The totp code must be 6 characters long.")]
    public required string Totp { get; set; }

    /// <summary>
    /// Enable or disable MFA of the totp code is successfully verified
    /// </summary>
    [Required]
    public bool EnableMfa { get; set; }
}