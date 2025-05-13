using System.ComponentModel.DataAnnotations;

namespace CleanIAM.Identity.Api.Views.PasswordReset;

public class PasswordResetRequestViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}