# File Organization

In the CleanIAM project, proper file organization is crucial for maintaining a clean, understandable codebase. The project follows consistent patterns for organizing files within each slice and for structuring the content of individual files.

## Single Responsibility Principle

Each file in the CleanIAM project typically contains a single primary type (class, record, or interface) with the same name as the file. This approach embodies the Single Responsibility Principle and makes the codebase easier to navigate and maintain.

For example:

- `UserService.cs` contains only the `UserService` class
- `ITokenValidator.cs` contains only the `ITokenValidator` interface
- `User.cs` contains only the `User` entity

## Command and Query Files

There is one notable exception to the single-type-per-file rule: command/query handlers are grouped with their corresponding command/query in the same file. This convention acknowledges their tight coupling while keeping related code together.

For example, a file named `CreateUserCommand.cs` would typically contain:

```csharp
// The command record
public record CreateUserCommand(string Username, string Email, string Password);

// The command handler
public class CreateUserCommandHandler
{
    public static async Task<Result> LoadAsync(CreateUserCommand command,
        IUserService userService)
    {
        // Validation logic
        return Result.Ok();
    }

    public static async Task<Result<UserCreatedEvent>> HandleAsync(
        CreateUserCommand command,
        Result loadResult,
        IUserService userService)
    {
        // Handler implementation
        // ...
    }
}
```

This approach keeps tightly coupled components together, making it easier to understand and modify related functionality.

## Directory Organization

Files are organized into directories according to both their architectural layer (Api, Application, Core, Infrastructure) and their functional purpose within each layer. This dual-axis organization helps developers quickly locate specific components:

1. **Layer-based organization**: Files are first organized by architectural layer
2. **Function-based organization**: Within each layer, files are organized by their functional purpose

For example, in the Application layer of a slice:

- `/Commands` contains all command-related files
- `/Queries` contains all query-related files
- `/EventHandlers` contains all event handler files
- `/Interfaces` contains service interfaces

This organization strikes a balance between enforcing architectural boundaries and grouping related functionality.

## Project References

Project references in CleanIAM are carefully managed to enforce the dependency rules of clean architecture:

1. Core projects have no dependencies on other projects
2. Application projects depend only on Core projects
3. Infrastructure projects depend on both Core and Application projects
4. API projects depend on all other projects

These reference constraints help maintain the integrity of the architectural layers and prevent inappropriate coupling between components.

## Benefits

This consistent approach to file organization provides several benefits:

1. **Improved productivity**: Developers can quickly locate files based on predictable patterns
2. **Better maintainability**: Related code is kept together while excessive coupling is prevented
3. **Easier onboarding**: New team members can quickly understand where different types of components belong
4. **Architectural integrity**: The physical organization of files reinforces the logical architecture of the system

By following these organizational principles, the CleanIAM project maintains a clean, understandable codebase that remains maintainable as it grows in complexity.
