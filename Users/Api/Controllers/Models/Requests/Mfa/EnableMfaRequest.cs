namespace Users.Api.Controllers.Models.Requests.Mfa;

/// <summary>
/// Response model for enabling/disabling MFA
/// </summary>
public class EnableMfaRequest
{
    /// <summary>
    /// Enable or disable MFA
    /// </summary>
    public bool Enable { get; set; }
}