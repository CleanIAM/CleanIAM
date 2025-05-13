using System.ComponentModel.DataAnnotations;
using CleanIAM.Identity.Infrastructure.Services;

namespace CleanIAM.Identity.Api.ViewModels.Signup;

public class SignupViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Length(2, 64, ErrorMessage = "First name length must be between 2 and 64 character long")] 
    public string FirstName { get; set; }

    [Required] 
    [Length(2, 64, ErrorMessage = "Last name length must be between 2 and 64 character long")] 
    public string LastName { get; set; }

    [Required]
    [CustomValidation(typeof(PasswordValidator), "ValidateAttribute")]
    public string Password { get; set; }
}