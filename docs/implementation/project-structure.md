# Project Structure

The CleanIAM project follows a well-defined directory structure that reflects its architectural principles. This section details how files and directories are organized throughout the codebase.

## Repository Directory Structure

At the root of the directory structure, there is a main dotnet solution file `CleanIAM.sln`, a main readme file `ReadMe.md` with important project information, and the `docs` directory containing the documentation files. The main source code is contained in the `src` directory, which contains all of the source files for each slice.

```
root of the project
├── docs                  // Documentation files
├── src                   // Source code directory
├── CleanIAM.sln          // Main solution file
└── ReadMe.md             // Project overview and instructions
```

## Slice Directory Structure

Each slice follows a consistent directory structure based on the clean architecture principles. This organization helps maintain separation of concerns and ensures that dependencies flow inward, from infrastructure to core domain.

```
Slice                     // Root folder for the slice
├── Api                   // API layer
│   ├── Controllers       // API controllers
│   ├── Models            // API models
│   └── Views             // Views and view models
├── Application           // Application layer
│   ├── Commands          // Commands and their handlers
│   ├── EventHandlers     // Handlers for domain events
│   ├── Interfaces        // Service interfaces
│   └── Queries           // Queries and their handlers
├── Core                  // Core domain layer
│   └── Events            // Domain events
└── Infrastructure        // Infrastructure layer
    ├── AnticorruptionLayer // Mapping for external events
    └── Services          // Implementations of interfaces
```

### API Layer

The `Api` directory contains everything that the end user can come in contact with, such as:

- `Controllers`: ASP.NET Core controllers defining HTTP endpoints
- `Models`: Request and response models used by controllers
- `Views`: In the case of the Identity slice, the MVC views rendered for users

### Application Layer

The `Application` directory contains the implementation of the business logic:

- `Commands`: Contains command objects and their handlers, implementing the CQRS pattern
- `EventHandlers`: Contains handlers for domain events emitted by other slices
- `Interfaces`: Contains service interfaces that define functionality required by the application
- `Queries`: Contains query objects and their handlers for data retrieval operations

### Core Layer

The `Core` directory contains the core domain model:

- Domain aggregates are placed directly in the root of this directory
- `Events`: Contains definitions of slice-specific domain events

### Infrastructure Layer

The `Infrastructure` directory contains implementation details and external system integrations:

- `AnticorruptionLayer`: Contains mappers that translate between local and global events
- `Services`: Contains implementations of interfaces defined in the Application layer

This structured approach ensures that each slice is well-organized, with clear boundaries between layers and a consistent pattern that developers can easily understand and navigate.
