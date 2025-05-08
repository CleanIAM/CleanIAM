using Applications.Core.Events;
using Mapster;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SharedKernel.Core.Database;
using SharedKernel.Infrastructure.Utils;
using Wolverine;

namespace Applications.Application.Commands;

/// <summary>
/// Command to delete OpenId application
/// </summary>
/// <param name="Id">Id of the application</param>
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
            return Result.Error($"Client with ID {command.Id} not found", 400);

        return Result.Ok(application);
    }

    public static async Task<Result<OpenIdApplicationDeleted>> Handle(DeleteOpenIdApplicationCommand command,
        Result<OpenIddictEntityFrameworkCoreApplication<Guid>> loadResult,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager,
        ApplicationDbContext dbContext,
        IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);

        try
        {
            // Since the applicationManager.DeleteAsync throws exception, that will be fixed in next version.
            // Just direct db delete is executed 

            await dbContext.OpenIddictEntityFrameworkCoreApplication
                .Where(t => t.Id == command.Id)
                .ExecuteDeleteAsync(cancellationToken);

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