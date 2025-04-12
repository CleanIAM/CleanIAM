using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using SharedKernel.Infrastructure;

namespace ManagementPortal.Application.Commands.OpenIdApplications;

public record UpdateOpenIdApplicationCommand(
    Guid Id,
    ApplicationType? ApplicationType,
    string ClientId,
    ClientType? ClientType,
    ConsentType? ConsentType,
    string? DisplayName,
    HashSet<string> Permissions,
    HashSet<Uri> PostLogoutRedirectUris,
    HashSet<Uri> RedirectUris,
    HashSet<string> Requirements,
    Dictionary<string, string> Settings);

public class UpdateOpenIdClientCommandHandler
{
    public static async Task<Result<OpenIddictEntityFrameworkCoreApplication<Guid>>> LoadAsync(UpdateOpenIdApplicationCommand command,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication<Guid>> applicationManager, CancellationToken cancellationToken)
    {
        // Find application by its Guid Id property
        var application = await applicationManager.FindByIdAsync(command.Id.ToString(), cancellationToken);
        if (application is null)
        {
            return Result<OpenIddictEntityFrameworkCoreApplication<Guid>>.Error($"Client with ID {command.Id} not found", 400);
        }   
        
        return Result<OpenIddictEntityFrameworkCoreApplication<Guid>>.Ok(application);
    }
    
    public static async Task<Result> Handle(UpdateOpenIdApplicationCommand command,
        Result<OpenIddictEntityFrameworkCoreApplication<Guid>> result, 
        IOpenIddictApplicationManager applicationManager, CancellationToken cancellationToken)
    {
        if (result.IsError())
            return result.Adapt<Result>();

        var descriptor = command.Adapt<OpenIddictApplicationDescriptor>();
        
        try
        {
            await applicationManager.UpdateAsync(result.Value, descriptor, cancellationToken);
            return Result.Ok();
        }
        catch (OpenIddictExceptions.ValidationException ex)
        {
            return Result.Error(ex.Message, 400);
        }
    }
}