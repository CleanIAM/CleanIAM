using ManagementPortal.Core;
using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure;

namespace ManagementPortal.Application.Commands;

public record UpdateOpenIdClientCommand(OpenIdApplication Client);

public class UpdateOpenIdClientCommandHandler
{
    public static async Task<Result<object>> LoadAsync(UpdateOpenIdClientCommand command,
        IOpenIddictApplicationManager applicationManager, CancellationToken cancellationToken)
    {
        // Find application by its Guid Id property
        var application = await applicationManager.FindByIdAsync(command.Client.Id.ToString(), cancellationToken);
        if (application is null)
        {
            return Result<object>.Error($"Client with ID {command.Client.Id} not found", 400);
        }   
        
        return Result<object>.Ok(application);
    }
    
    public static async Task<Result> Handle(UpdateOpenIdClientCommand command, Result<object> result, 
        IOpenIddictApplicationManager applicationManager, CancellationToken cancellationToken)
    {
        if (result.IsError())
        {
            return result.Adapt<Result>();
        }
        var application = result.Value;
        
        var descriptor = command.Client.Adapt<OpenIddictApplicationDescriptor>();
        
        try
        {
            // Update the application
            await applicationManager.UpdateAsync(application, descriptor, cancellationToken);
            return Result.Ok();
        }
        catch (OpenIddictExceptions.ValidationException ex)
        {
            return Result.Error(ex.Message, 400);
        }
    }
}