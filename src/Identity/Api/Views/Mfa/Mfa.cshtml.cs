using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Views.Mfa;

/// <summary>
/// ViewModel for handling Multi-Factor Authentication (MFA) input.
/// </summary>
public class MfaViewModel
{
    /// <summary>
    /// The totp code from the Users authenticator app.
    /// </summary>
    [Required]
    [Length(6, 6, ErrorMessage = "The code must be 6 characters long.")]
    public string Totp { get; set; } = string.Empty;
}