using System.ComponentModel.DataAnnotations;
using Identity.Infrastructure.Services;

namespace Identity.Api.ViewModels.Signup;

public class SignupViewModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] [MinLength(3)] public string FirstName { get; set; }

    [Required] [MinLength(3)] public string LastName { get; set; }

    [Required]
    [CustomValidation(typeof(PasswordValidator), "ValidateAttribute")]
    public string Password { get; set; }
}