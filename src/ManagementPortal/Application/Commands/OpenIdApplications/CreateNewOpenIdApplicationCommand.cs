using System.Net;
using ManagementPortal.Core.Events.OpenIdApplications;
using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using Marten;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SharedKernel.Infrastructure;
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
    HashSet<string> Endpoints,
    HashSet<string> GrantTypes,
    HashSet<string> ResponseTypes,
    HashSet<Uri> PostLogoutRedirectUris,
    HashSet<Uri> RedirectUris,
    HashSet<string> Requirements,
    Dictionary<string, string> Settings
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

        //TODO: Create new app
        var application = command.Adapt<OpenIdApplication>();
        
        // Generate a client secret only if the application type is confidential
        if (application.ClientType == ClientType.Confidential)
            application.ClientSecret = Guid.NewGuid().ToString();
            

        var descriptor = application.ToDescriptor();

        try
        {
            await applicationManager.CreateAsync(descriptor);

            var applicationCreatedEvent = application.Adapt<OpenIdApplicationCreated>();
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