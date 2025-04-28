using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Mails;

/// <summary>
/// Object representing the recipient of the email.
/// </summary>
public class MailReceiver
{
    [EmailAddress]
    public required string Email { get; set; }
    
    public required string Name { get; set; }
}