using Applications.Core;
using Applications.Core.Events;
using Mapster;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace Applications.Application.Commands;

/// <summary>
/// Command to update OpenId application
/// </summary>
/// <param name="Id"></param>
/// <param name="ApplicationType"></param>
/// <param name="ClientId"></param>
/// <param name="ClientType"></param>
/// <param name="ConsentType"></param>
/// <param name="DisplayName"></param>
/// <param name="Scopes"></param>
/// <param name="PostLogoutRedirectUris"></param>
/// <param name="RedirectUris"></param>
public record UpdateOpenIdApplicationCommand(
    Guid Id,
    ApplicationType? ApplicationType,
    string ClientId,
    ClientType? ClientType,
    ConsentType? ConsentType,
    string? DisplayName,
    HashSet<string> Scopes,
    HashSet<Uri> PostLogoutRedirectUris,
    HashSet<Uri> RedirectUris);

public class UpdateOpenIdClientCommandHandler
{
    public static async Task<Result<OpenIddictEntityFrameworkCoreApplication<Guid>>> LoadAsync(
        UpdateOpenIdApplicationCommand command,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager,
        CancellationToken cancellationToken)
    {
        // Find application by its Guid Id property
        var application = await applicationManager.FindByIdAsync(command.Id.ToString(), cancellationToken);
        if (application is null)
            return Result.Error($"Client with ID {command.Id} not found", 400);

        return Result.Ok(application);
    }

    public static async Task<Result<OpenIdApplicationUpdated>> Handle(UpdateOpenIdApplicationCommand command,
        Result<OpenIddictEntityFrameworkCoreApplication<Guid>> loadResult,
        IMessageBus bus,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager
        , CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);

        var descriptor = command.Adapt<OpenIddictApplicationDescriptor>();

        // Since the client secret is hidden in the UI, we need to load it from the existing application
        descriptor.ClientSecret = loadResult.Value.ClientSecret;

        // Allow application to use auth endpoint
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.EndSession);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);

        try
        {
            await applicationManager.UpdateAsync(loadResult.Value, descriptor, cancellationToken);

            // On success publish event and return Ok with that event
            var applicationUpdatedEvent = command.Adapt<OpenIdApplicationUpdated>();
            await bus.PublishAsync(applicationUpdatedEvent);
            return Result.Ok(applicationUpdatedEvent);
        }
        catch (OpenIddictExceptions.ValidationException ex)
        {
            return Result.Error(ex.Message, 400);
        }
    }
}