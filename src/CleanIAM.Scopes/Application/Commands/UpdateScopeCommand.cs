using CleanIAM.Scopes.Core.Events;
using Mapster;
using OpenIddict.Core;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using Wolverine;
using OpenIddictScope = OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreScope<System.Guid>;

namespace CleanIAM.Scopes.Application.Commands;

/// <summary>
/// Command to update existing scope.
/// </summary>
/// <param name="Name">Name of the scope to update</param>
/// <param name="DisplayName">New display name of the scope</param>
/// <param name="Description">New description name of the scope</param>
/// <param name="Resources">Resources the scope allows access to</param>
public record UpdateScopeCommand(string Name, string DisplayName, string Description, string[] Resources);

public class UpdateScopeCommandHandler
{
    public static async Task<Result<OpenIddictScope>> LoadAsync(UpdateScopeCommand command,
        OpenIddictScopeManager<OpenIddictScope> scopeManager, CancellationToken cancellationToken)
    {
        // Check if the scope with the given name isn't default scope
        if (ScopesConstants.DefaultScopeNames.Contains(command.Name))
            return Result.Error("Default scopes cannot be updated", StatusCodes.Status400BadRequest);

        // Check if the scope with the given name doesn't exist
        var scope = await scopeManager.FindByNameAsync(command.Name, cancellationToken);
        if (scope is null)
            return Result.Error("Scope not found", StatusCodes.Status400BadRequest);

        return Result.Ok(scope);
    }

    public static async Task<Result<ScopeUpdated>> HandleAsync(UpdateScopeCommand command,
        Result<OpenIddictScope> loadResult,
        OpenIddictScopeManager<OpenIddictScope> scopeManager, IMessageBus bus, CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return Result.From(loadResult);
        var scope = loadResult.Value;

        var newScope = command.Adapt(scope);

        try
        {
            await scopeManager.UpdateAsync(newScope, cancellationToken);
        }
        catch
        {
            return Result.Error("Error updating scope", StatusCodes.Status500InternalServerError);
        }

        // Publish the ScopeUpdated event and return
        var scopeUpdatedEvent = command.Adapt<ScopeUpdated>();
        await bus.PublishAsync(scopeUpdatedEvent);
        return Result.Ok(scopeUpdatedEvent);
    }
}