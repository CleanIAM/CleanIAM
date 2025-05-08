namespace Users.Api.Controllers.Models.Responses.Mfa;

/// <summary>
/// Response model for MFA configuration flow
/// </summary>
public class MfaConfigurationResponse
{
    /// <summary>
    /// QrCode with totp configuration string
    /// Represents PNG image in Base64 format
    /// </summary>
    public required string QrCode { get; set; }
}