# Naming Conventions

Consistent naming conventions are essential for maintaining code readability and ensuring that developers can quickly understand code without extensive documentation. The CleanIAM project follows specific naming conventions aligned with both .NET community standards and the specific architectural patterns used in the project.

## General Naming Conventions

### Case Conventions

- **PascalCase**: Used for all public types, members, and constants
  - Classes: `UserService`, `AuthorizationController`
  - Public methods: `GetUserById`, `ValidateToken`
  - Public properties: `UserId`, `FirstName`
- **camelCase**: Used for private/protected fields and local variables
  - Private fields: `private string _userName;`
  - Local variables: `var userResponse = await _userService.GetUserAsync();`

### Prefixes and Suffixes

- **Interface names** begin with "I": `IUserService`, `ITokenValidator`
- **Abstract classes** may include "Base" or "Abstract": `BaseEntity`, `AbstractValidator`
- **Extension classes** end with "Extensions": `StringExtensions`, `EnumerableExtensions`

## Domain-Specific Naming

::: warning
Commands, Queries, and Events and their handlers **must** be name according to the following conventions otherwise they will not be registered by WolverineFx.
:::

### Commands and Queries

- **Commands** use a verb-noun format and end with "Command":
  - `CreateUserCommand`, `UpdateProfileCommand`, `DeleteClientCommand`
- **Queries** use a descriptive format and end with "Query":
  - `GetUserByIdQuery`, `ListActiveUsersQuery`, `SearchClientsQuery`
- **Handlers** append "Handler" to the command or query name:
  - `CreateUserCommandHandler`, `GetUserByIdQueryHandler`

### Events

- **Events** use past tense to indicate they represent something that has happened:
  - `UserCreated`, `ClientDeleted`, `PasswordChanged`
- **Event handlers** append "EventHandler" to the event name:
  - `UserCreatedEventHandler`, `ClientDeletedEventHandler`

### Services

- **Services** are named based on their domain and responsibility:
  - `UserService`, `TokenService`, `EmailService`

## File Naming Conventions

Each file in the CleanIAM project should contain a single primary class, record, or interface with the same name as the file:

- `UserService.cs` contains the `UserService` class
- `ITokenValidator.cs` contains the `ITokenValidator` interface
- `CreateUserCommand.cs` contains both the `CreateUserCommand` record and its handler

This convention makes it easy to locate specific types and ensures a clean, organized codebase.

## Summary

Following these naming conventions ensures that the CleanIAM codebase remains consistent and that new developers can quickly understand the purpose and responsibility of each component without extensive documentation. Consistent naming is especially important in a project with a vertical slice architecture, where similar patterns are repeated across different slices of functionality.
