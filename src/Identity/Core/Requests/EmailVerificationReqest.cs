using Marten.Schema;

namespace Identity.Core.Requests;

[SingleTenanted]
public class EmailVerificationReqest
{
    /// <summary>
    /// Id of the request
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id of the user that requested the email verification
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Email of the user that requested the email verification
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// First name of the user that requested the email verification
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Last name of the user that requested the email verification
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// When was the last email verification email send
    /// </summary>
    public DateTime LastEmailsSendAt { get; set; }
}