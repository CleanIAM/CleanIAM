namespace Users.Api.Controllers.Models.Responses.Mfa;

/// <summary>
/// Response model for enabling/disabling MFA
/// </summary>
public class MfaUpdatedResponse
{
    /// <summary>
    /// Indicates whether MFA is enabled or not
    /// </summary>
    public bool MfaEnabled { get; set; }
}