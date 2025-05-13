using CleanIAM.SharedKernel.Application.Interfaces.Utils;
using OtpNet;

namespace CleanIAM.SharedKernel.Infrastructure.Utils;

/// <summary>
/// Implementation of ITotpValidator for validating TOTP codes and generating TOTP configuration URLs.
/// </summary>
public class TotpValidator : ITotpValidator
{
    /// <summary>
    /// Validates the TOTP code against the secret key.
    /// </summary>
    /// <param name="totp">TOTP to validate</param>
    /// <param name="totpSecretKey">User's TOTP configuration secret</param>
    /// <returns></returns>
    public Result ValidateTotp(string totp, string totpSecretKey)
    {
        // Convert the stored Base32 secret back into bytes
        var keyBytes = Base32Encoding.ToBytes(totpSecretKey);
        var totpTool = new Totp(keyBytes);

        // Verification allows for a small window of time steps
        var res = totpTool.VerifyTotp(
            totp,
            out _,
            // How many time windows in the past and future is the code valid for
            new VerificationWindow(1));

        if (res)
            return Result.Ok();
        return Result.Error("Invalid TOTP code");
    }

    /// <summary>
    /// Generates a TOTP configuration URL for the user.
    /// </summary>
    /// <param name="email">CleanIAM.Users email</param>
    /// <returns>(ConfigurationUrl, The new user's TOTP configuration secret)</returns>
    public (string url, string totpSecretKey) GenerteTotpConfigurationUrl(string email)
    {
        var totpSecretKey = GenerateSecretKey();
        var issuer = SharedKernelConstants.MfaIssuer;
        return (
            url: $"otpauth://totp/{issuer}:{email}?secret={totpSecretKey}&issuer={issuer}",
            totpSecretKey
        );
    }

    /// <summary>
    /// Helper method to generate a random secret key for MFA.
    /// </summary>
    /// <returns></returns>
    private static string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }
}