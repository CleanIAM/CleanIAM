using Mapster;
using OpenIddict.Core;
using Scopes.Core.Events;
using SharedKernel.Infrastructure.Utils;
using Wolverine;
using OpenIddictScope = OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreScope<System.Guid>;

namespace Scopes.Application.Commands;

/// <summary>
/// Command to create a new scope.
/// </summary>
/// <param name="Name">Name of the new scope</param>
/// <param name="DisplayName">Display name of the new scope</param>
/// <param name="Description">Description name of the new scope</param>
/// <param name="Resources">Resources the scope allows access to</param>
public record CreateNewScopeCommand(string Name, string DisplayName, string Description, string[] Resources);

public class CreateNewScopeCommandHandler
{
    public static async Task<Result> LoadAsync(CreateNewScopeCommand command,
        OpenIddictScopeManager<OpenIddictScope> scopeManager, CancellationToken cancellationToken)
    {
        // Check if the scope with the given name doesn't exist
        var scope = await scopeManager.FindByNameAsync(command.Name, cancellationToken);
        if (scope is not null)
            return Result.Error("Scope with given name already exists", StatusCodes.Status400BadRequest);

        return Result.Ok();
    }

    public static async Task<Result<ScopeCreated>> HandleAsync(CreateNewScopeCommand command, Result loadResult,
        OpenIddictScopeManager<OpenIddictScope> scopeManager, IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return loadResult;

        var newScope = command.Adapt<OpenIddictScope>();
        try
        {
            await scopeManager.CreateAsync(newScope, cancellationToken);
        }
        catch
        {
            return Result.Error("Error creating scope", StatusCodes.Status500InternalServerError);
        }

        // Publish the ScopeCreated event and return
        var scopeCreatedEvent = command.Adapt<ScopeCreated>();
        await bus.PublishAsync(scopeCreatedEvent);
        return Result.Ok(scopeCreatedEvent);
    }
}