using CleanIAM.Identity.Application.Interfaces;
using CleanIAM.Identity.Core.Events;
using CleanIAM.Identity.Core.Users;
using Mapster;
using Marten;
using CleanIAM.SharedKernel.Core;
using CleanIAM.SharedKernel.Infrastructure.Utils;
using Wolverine;
using HashedPassword = CleanIAM.Identity.Core.Users.HashedPassword;
using Users_HashedPassword = CleanIAM.Identity.Core.Users.HashedPassword;

namespace CleanIAM.Identity.Application.Commands.Users;

public record CreateNewUserCommand(string Email, string FirstName, string LastName, string Password);

public class CreateNewUserCommandHandler
{
    public static async Task<Result> LoadAsync(CreateNewUserCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        // Normalize email
        var normalizedEmail = command.Email.ToLowerInvariant();

        var user = await session.Query<IdentityUser>()
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
        if (user is not null)
            return Result.Error("User with given email already exists", 400);

        return Result.Ok();
    }

    public static async Task<Result<NewUserSignedUp>> HandleAsync(CreateNewUserCommand command, Result loadResult,
        IDocumentSession documentSession, IPasswordHasher passwordHasher, IMessageBus bus,
        CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return loadResult;

        var password = passwordHasher.Hash(command.Password);
        var newUser = command.Adapt<IdentityUser>();
        newUser.Email = command.Email.ToLowerInvariant(); // Normalize email
        newUser.Id = Guid.NewGuid();
        newUser.Roles = [UserRole.User];
        newUser.HashedPassword = password.Adapt<Users_HashedPassword>();

        documentSession.Store(newUser);
        await documentSession.SaveChangesAsync(cancellationToken);

        // Publish user signed up event
        var userSignedUp = newUser.Adapt<NewUserSignedUp>();
        await bus.PublishAsync(userSignedUp, new DeliveryOptions{TenantId = newUser.TenantId.ToString()});

        return Result.Ok(userSignedUp);
    }
}