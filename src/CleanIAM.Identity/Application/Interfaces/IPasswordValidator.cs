using System.ComponentModel.DataAnnotations;
using FluentValidation;
using CleanIAM.SharedKernel.Infrastructure.Utils;

namespace CleanIAM.Identity.Application.Interfaces;

/// <summary>
/// Validate if the password is strong enough or contains required characters
/// </summary>
public interface IPasswordValidator
{
    /// <summary>
    /// Base function to validate password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static abstract Result Validate(string password);

    /// <summary>
    /// This is implementation of helper function for fluentValidation password validation.
    /// </summary>
    /// <param name="password">password</param>
    /// <param name="context">Fluent validation context</param>
    /// <typeparam name="T"></typeparam>
    public static abstract void Validate<T>(string password, ValidationContext<T> context);

    /// <summary>
    /// Method to validate passwords with [System.ComponentModel.DataAnnotations.CustomValidation()] attribute
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static abstract ValidationResult? ValidateAttribute(string password);
}