namespace Identity.Core;

public class PasswordResetRequest
{
    /// <summary>
    /// Id of the request
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id of the user that requested the password reset
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Email of the user that requested the password reset
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// First name of the user that requested the password reset
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Last name of the user that requested the password reset
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// When was the last password reset email send
    /// </summary>
    public DateTime LastEmailsSendAt { get; set; }
}