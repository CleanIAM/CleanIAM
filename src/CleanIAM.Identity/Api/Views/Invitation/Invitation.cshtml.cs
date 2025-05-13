using System.ComponentModel.DataAnnotations;
using CleanIAM.Identity.Infrastructure.Services;

namespace CleanIAM.Identity.Api.Views.Invitation;

public class InvitationViewModel
{
    [Required]
    public Guid RequestId { get; set; }

    [Required]
    [CustomValidation(typeof(PasswordValidator), "ValidateAttribute")]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}