using System.Net;
using ManagementPortal.Core.Events.Mfa;
using Marten;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace ManagementPortal.Api.Controllers.Models.Requests.Mfa;

/// <summary>
/// Remove mfa configuration command for user and disable mfa.
/// </summary>
/// <param name="Id"></param>
public record CleanMfaConfigurationCommand(Guid Id);

public class CleanMfaConfigurationCommandHandler
{
    public static async Task<Result<Core.Users.User>> LoadAsync(CleanMfaConfigurationCommand command,
        IQuerySession session, CancellationToken cancellationToken)
    {
        var user = await session.LoadAsync<Core.Users.User>(command.Id, cancellationToken);

        if (user is null)
            return Result.Error("User not found", HttpStatusCode.NotFound);

        return Result.Ok(user);
    }

    public static async Task<Result<MfaConfiguredForUser>> Handle(CleanMfaConfigurationCommand command,
        Result<Core.Users.User> loadResult, IDocumentSession session, IMessageBus bus,
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