using System.Net;
using ManagementPortal.Core.Users;
using Marten;
using QRCoder;
using SharedKernel.Application.Interfaces.Utils;
using SharedKernel.Infrastructure.Utils;

namespace ManagementPortal.Application.Commands.Mfa;

/// <summary>
/// Command to generate MFA setup qrcode (qrCode is encoded in BASE64 string)
/// </summary>
/// <param name="Id">Id of the user to generate qr for</param>
public record GenerateMfaConnectionQrCodeCommand(Guid Id);

/// <summary>
/// Handler class for GenerateMfaConnectionQrCodeCommand
/// </summary>
public class GenerateMfaConnectionQrCodeCommandHandler
{
    public static async Task<Result<User>> LoadAsync(GenerateMfaConnectionQrCodeCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        var user = await session.LoadAsync<User>(command.Id, cancellationToken);

        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok(user);
    }

    /// <summary>
    /// Handle GenerateMfaConnectionQrCodeCommand
    /// </summary>
    /// <returns>qrCode is encoded in BASE64 string with encoded totp connection url</returns>
    public static async Task<Result<string>> HandleAsync(GenerateMfaConnectionQrCodeCommand command,
        Result<User> loadResult, IDocumentSession session, ITotpValidator totpValidator,
        CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        var (mfaConnectionUrl, secret) = totpValidator.GenerteTotpConfigurationUrl(user.Email);

        // Update user with the new secret key in database
        user.MfaConfig.TotpSecretKey = secret;
        // Mfa is not configured yet, the user need to validate the configuration process
        user.MfaConfig.IsMfaConfigured = false;
        session.Update(user);
        await session.SaveChangesAsync(cancellationToken);

        // Generate qrcode form url
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(mfaConnectionUrl, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);

        // Convert qrCode image to base64
        var bitmap = qrCode.GetGraphic(20);
        var base64Image = Convert.ToBase64String(bitmap);
        return Result.Ok($"data:image/png;base64,{base64Image}");
    }
}