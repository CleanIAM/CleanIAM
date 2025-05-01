using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Identity.Application.Interfaces;
using SharedKernel.Infrastructure;

namespace Identity.Infrastructure.Services;

public class PasswordValidator : IPasswordValidator
{
    public static Result Validate(string password)
    {
        //TODO: Verify password correctly
        if (string.IsNullOrWhiteSpace(password))
            return Result.Error("Password cannot be empty");

        if (password.Length < 8)
            return Result.Error("Password must be at least 8 characters long");

        if (!password.Any(char.IsUpper))
            return Result.Error("Password must contain at least one uppercase letter");

        if (!password.Any(char.IsLower))
            return Result.Error("Password must contain at least one lowercase letter");

        if (!password.Any(char.IsDigit))
            return Result.Error("Password must contain at least one digit");

        if (!password.Any(c => "!@#$%^&*()_+-=[]{}|;':\",.<>?/`~".Contains(c)))
            return Result.Error("Password must contain at least one special character");

        return Result.Ok();
    }

    public static void Validate<T>(string password, ValidationContext<T> context)
    {
        var res = Validate(password);
        if (res.IsError())
            context.AddFailure(res.ErrorValue.Message);
    }

    public static ValidationResult? ValidateAttribute(string password)
    {
        var res = Validate(password);
        return res.IsError() ? new ValidationResult(res.ErrorValue.Message) : ValidationResult.Success;
    }
}