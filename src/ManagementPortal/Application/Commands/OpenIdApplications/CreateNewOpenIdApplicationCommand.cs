using System.Net;
using ManagementPortal.Core.Events.OpenIdApplications;
using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using Marten;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace ManagementPortal.Application.Commands.OpenIdApplications;

public record CreateNewOpenIdApplicationCommand(
    Guid Id,
    ApplicationType? ApplicationType,
    string ClientId,
    ClientType? ClientType,
    ConsentType? ConsentType,
    string? DisplayName,
    HashSet<string> Scopes,
    HashSet<Uri> PostLogoutRedirectUris,
    HashSet<Uri> RedirectUris
);

public class CreateNewOpenIdApplicationCommandHandler
{
    public static async Task<Result> LoadAsync(CreateNewOpenIdApplicationCommand command, IQuerySession session,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager)
    {
        // Since the clientId must be unique for all application we need to check it 
        var app = await applicationManager.FindByClientIdAsync(command.ClientId);

        if (app != null)
            return Result.Error("Application with given ClientId already exist!", HttpStatusCode.BadRequest);

        return Result.Ok();
    }

    public static async Task<Result<OpenIdApplicationCreated>> Handle(CreateNewOpenIdApplicationCommand command,
        Result loadResult, IMessageBus bus,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager
    )
    {
        if (loadResult.IsError())
            return loadResult;

        var descriptor = command.Adapt<OpenIddictApplicationDescriptor>();


        // Generate a client secret only if the application type is confidential
        if (command.ClientType == ClientType.Confidential)
            descriptor.ClientSecret = Guid.NewGuid().ToString();

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
            await applicationManager.CreateAsync(descriptor);

            var applicationCreatedEvent = command.Adapt<OpenIdApplicationCreated>() with
            {
                ClientSecret = descriptor.ClientSecret
            };
            await bus.PublishAsync(applicationCreatedEvent);
            return Result.Ok(applicationCreatedEvent);
        }
        catch (Exception e)
        {
            return Result.Error("Creating new application failed",
                HttpStatusCode.InternalServerError);
        }
    }
}