using System.Net;
using Marten;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.Users.Core;
using CleanIAM.Users.Core.Events.Mfa;
using Wolverine;

namespace CleanIAM.Users.Application.Commands.Mfa;

/// <summary>
/// Remove mfa configuration command for user and disable mfa.
/// </summary>
/// <param name="Id">Id of the user</param>
public record CleanMfaConfigurationCommand(Guid Id);

public class CleanMfaConfigurationCommandHandler
{
    public static async Task<Result<User>> LoadAsync(CleanMfaConfigurationCommand command,
        IQuerySession session, CancellationToken cancellationToken)
    {
        var user = await session.LoadAsync<User>(command.Id, cancellationToken);

        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok(user);
    }

    public static async Task<Result<MfaConfiguredForUser>> HandleAsync(CleanMfaConfigurationCommand command,
        Result<User> loadResult, IDocumentSession session, IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var user = loadResult.Value;

        user.IsMFAEnabled = false;
        user.MfaConfig.IsMfaConfigured = false;
        user.MfaConfig.TotpSecretKey = string.Empty;
        session.Update(user);
        await session.SaveChangesAsync(cancellationToken);

        var mfaConfiguredForUser = new MfaConfiguredForUser(user.Id, string.Empty, false);
        await bus.PublishAsync(mfaConfiguredForUser);
        return Result.Ok(mfaConfiguredForUser);
    }
}