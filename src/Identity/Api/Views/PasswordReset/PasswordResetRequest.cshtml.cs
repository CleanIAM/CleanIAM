using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Views.PasswordReset;

public class PasswordResetRequestViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}