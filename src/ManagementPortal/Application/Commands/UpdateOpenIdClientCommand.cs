using ManagementPortal.Core;
using Mapster;
using Marten;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure;

namespace ManagementPortal.Application.Commands;

public record UpdateOpenIdClientCommand(OpenIdApplication Client);

public class UpdateOpenIdClientCommandHandler
{

    public static async Task<Result<object>> LoadAsync(UpdateOpenIdClientCommand command,
        IOpenIddictApplicationManager applicationManager, CancellationToken cancellationToken)
    {
     
        var application = await applicationManager.FindByIdAsync(command.Client.ClientId, cancellationToken);
        if (application is null)
        {
            return Result<object>.Error("Client not found", 400);
        }   
        return Result<object>.Ok(application);
    }
    
    public static async Task<Result> Handle(UpdateOpenIdClientCommand command, Result<object> application, IOpenIddictApplicationManager applicationManager, CancellationToken cancellationToken)
    {
        if (application.IsError())
        {
            return Result.Error(application.ErrorValue.Message, application.ErrorValue.Code);
        }

        var updatedApplication = application.Adapt(command.Client);

        var validationResults = await applicationManager.ValidateAsync(updatedApplication, cancellationToken).ToListAsync(cancellationToken: cancellationToken);
        if (!validationResults.IsEmpty())
        {
            return Result.Error(validationResults[0].ErrorMessage, 400);
        }

        await applicationManager.UpdateAsync(updatedApplication, cancellationToken);
        
        return Result.Ok();
    }
}