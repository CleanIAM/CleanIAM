using Identity.Application.Interfaces;
using Identity.Core.Users;
using Mapster;
using Marten;
using SharedKernel.Core;
using SharedKernel.Infrastructure;

namespace Identity.Application.Commands.Users;

public record CreateNewUserCommand(string Email, string FirstName, string LastName, string Password);

public class CreateNewUserCommandHandler
{
    public static async Task<Result> LoadAsync(CreateNewUserCommand command, IQuerySession session,
        CancellationToken cancellationToken)
    {
        // Normalize email
        var normalizedEmail = command.Email.ToLowerInvariant();

        var user = await session.Query<User>().FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
        if (user is not null)
            return Result.Error("User with given email already exists", 400);

        return Result.Ok();
    }

    public static async Task<Result> Handle(CreateNewUserCommand command, Result loadResult,
        IDocumentSession documentSession, IPasswordHasher passwordHasher, CancellationToken cancellationToken)
    {
        if (loadResult.IsError())
            return loadResult;

        var password = passwordHasher.Hash(command.Password);
        var newUser = command.Adapt<User>();
        newUser.Email = command.Email.ToLowerInvariant(); // Normalize email
        newUser.Id = Guid.NewGuid();
        newUser.Roles = [UserRole.User];
        newUser.HashedPassword = password.Adapt<HashedPassword>();

        documentSession.Store(newUser);
        await documentSession.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}