using SharedKernel.Infrastructure.Utils;

namespace SharedKernel.Application.Interfaces.Utils;

/// <summary>
/// Utility to help with TOTP (Time-based One-Time Password) generation and validation.
/// This utility can be used to validate TOTP and also to generate the TOTP configuration URL
/// with which users can connect apps like Google Authenticator.
/// </summary>
public interface ITotpValidator
{
    /// <summary>
    /// Validates the TOTP code against the secret key.
    /// </summary>
    /// <param name="totp">TOTP to validate</param>
    /// <param name="totpSecretKey">User's TOTP configuration secret</param>
    /// <returns></returns>
    public Result ValidateTotp(string totp, string totpSecretKey);

    /// <summary>
    /// Generates a TOTP configuration URL for the user.
    /// </summary>
    /// <param name="email">Users email</param>
    /// <returns>(ConfigurationUrl, The new user's TOTP configuration secret)</returns>
    public (string url, string totpSecretKey) GenerteTotpConfigurationUrl(string email);
}