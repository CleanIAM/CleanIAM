# CQRS Pattern Implementation

The CleanIAM project implements the Command Query Responsibility Segregation (CQRS) pattern as a core architectural approach. This pattern separates read and write operations into distinct models, providing several benefits including improved scalability, better separation of concerns, and enhanced maintainability.

## Commands and Handlers

Commands in CleanIAM represent actions that change the state of the system. They follow a consistent naming pattern and structure:

### Command Structure

1. **Naming**: Commands are named with a verb phrase followed by "Command" (e.g., `CreateUserCommand`, `UpdateApplicationCommand`).
2. **Implementation**: Commands are implemented as C# records, which makes them immutable and provides value-based equality semantics.
3. **Properties**: Commands contain all the necessary data to execute the operation.

### Command Handler Structure

Command handlers follow these conventions:

1. **Naming**: Handlers are named by appending "Handler" to the command name (e.g., `CreateUserCommandHandler`).
2. **Methods**: Each handler implements two key methods:
   - `LoadAsync`: Responsible for loading any required data and performing initial validation
   - `HandleAsync`: Contains the business logic and publishes resulting events

Here's an example of the standard command handler pattern:

```csharp
public class ExampleCommandHandler
{
    public static async Task<Result> LoadAsync(ExampleCommand command,
        IExampleService service /* Injecting service from DI */)
    {
        return Result.Ok();
    }

    public static async Task<Result<ExampleEvent>> HandleAsync(ExampleCommand command,
        Result loadResult, IExampleService service /* Injecting service from DI */)
    {
        if (loadResult.IsError())
            return loadResult;

        // Perform the command logic here

        // Publish the ExampleEvent event and return
        var exampleEvent = command.Adapt<ExampleEvent>();
        await bus.PublishAsync(exampleEvent);
        return Result.Ok(exampleEvent);
    }
}
```

Key points about this pattern:

- Both methods are declared as `static` to work with the WolverineFx discovery mechanism
- The `LoadAsync` method returns a `Result` object, which is passed to the `HandleAsync` method
- Error checking is mandatory at the beginning of `HandleAsync`
- Dependency injection is handled via parameter injection
- The handler publishes domain events after successfully executing the command
- Command results are wrapped in a `Result` or `Result<T>` type to provide consistent error handling

## Queries and Handlers

Queries represent operations that retrieve data without modifying state. They follow similar conventions to commands:

### Query Structure

1. **Naming**: Queries end with "Query" (e.g., `GetUserQuery`, `ListApplicationsQuery`)
2. **Implementation**: Like commands, queries are implemented as records
3. **Properties**: Queries contain the parameters needed to retrieve the requested data

### Query Handler Structure

Query handlers follow these conventions:

1. **Naming**: Query handlers are named with "QueryHandler" suffix
2. **Methods**: Unlike command handlers, query handlers typically only implement the `HandleAsync` method
3. **Return Type**: Query handlers return data wrapped in a `Result<T>` type

Here's an example of a query handler:

```csharp
public class GetUserQueryHandler
{
    public static async Task<Result<UserDto>> HandleAsync(GetUserQuery query,
        IQuerySession session /* Injecting martens query session from DI */)
    {
        var user = await session.LoadAsync<User>(query.UserId);

        if (user == null)
            return Result.Error("User not found", 404);

        return Result.Ok(user.Adapt<UserDto>());
    }
}
```

## WolverineFx Integration

The CleanIAM project leverages the WolverineFx framework for implementing the CQRS pattern. WolverineFx provides several advantages:

1. **Automatic handler discovery**: WolverineFx automatically matches commands and queries with their handlers based on naming conventions.
2. **Dependency injection**: Dependencies are automatically injected into handlers.
3. **Pipeline behavior**: WolverineFx supports middleware for cross-cutting concerns.
4. **Message routing**: Events can be easily published and handled throughout the application.

The static methods approach used in the command and query handlers is specifically designed to work with WolverineFx's handler discovery mechanism, resulting in clean, decoupled code with minimal boilerplate.
