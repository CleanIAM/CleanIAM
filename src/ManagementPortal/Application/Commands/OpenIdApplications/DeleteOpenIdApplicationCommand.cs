using ManagementPortal.Core.Events.OpenIdApplications;
using Mapster;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Application.Commands.OpenIdApplications;

public record DeleteOpenIdApplicationCommand(Guid Id);

public class DeleteOpenIdApplicationCommandHandler
{
    public static async Task<Result<OpenIddictEntityFrameworkCoreApplication<Guid>>> LoadAsync(
        DeleteOpenIdApplicationCommand command,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager,
        CancellationToken cancellationToken)
    {
        // Find application by its Guid Id property
        var application = await applicationManager.FindByIdAsync(command.Id.ToString(), cancellationToken);
        if (application is null)
        {
            return Result.Error($"Client with ID {command.Id} not found", 400);
        }

        return Result.Ok(application);
    }

    public static async Task<Result<OpenIdApplicationDeleted>> Handle(DeleteOpenIdApplicationCommand command,
        Result<OpenIddictEntityFrameworkCoreApplication<Guid>> loadResult,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        
        try
        {
            await applicationManager.DeleteAsync(loadResult.Value, cancellationToken);
            
            // On success publish event and return Ok with that event
            var applicationDeletedEvent = command.Adapt<OpenIdApplicationDeleted>();
            await bus.PublishAsync(applicationDeletedEvent);
            return Result.Ok(applicationDeletedEvent);
        }
        catch (OpenIddictExceptions.ValidationException ex)
        {
            return Result.Error(ex.Message, 400);
        }
    }
}