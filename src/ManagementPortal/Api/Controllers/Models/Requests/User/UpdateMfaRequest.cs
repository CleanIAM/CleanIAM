namespace ManagementPortal.Api.Controllers.Models.Requests.User;

public class UpdateMfaRequest
{
    /// <summary>
    /// Enable or disable MFA
    /// </summary>
    public bool Enabled { get; set; }
}