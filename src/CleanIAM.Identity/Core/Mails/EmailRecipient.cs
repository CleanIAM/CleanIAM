using System.ComponentModel.DataAnnotations;

namespace CleanIAM.Identity.Core.Mails;

/// <summary>
/// Object representing the recipient of the email.
/// </summary>
public class EmailRecipient
{
    /// <summary>
    /// Email of the recipient
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// First name of the recipient
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Last name of the recipient
    /// </summary>
    public required string LastName { get; set; }
}