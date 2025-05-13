using System.ComponentModel.DataAnnotations;

namespace CleanIAM.Identity.Api.ViewModels.Signin;

public class SigninViewModel
{
    [Required] [EmailAddress] public required string Email { get; set; }

    [Required] [MaxLength(128)] public required string Password { get; set; }
}