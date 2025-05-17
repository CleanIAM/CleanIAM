# Vertical Slices

CleanIAM is designed using the Vertical Slice Architecture pattern, which organizes code by features or business capabilities rather than technical layers. This approach provides several benefits:

- **Improved cohesion**: Code related to a specific feature is kept together
- **Reduced coupling**: Features are isolated, minimizing dependencies between unrelated components
- **Easier maintenance**: Changes to a feature are localized to its slice
- **Better scalability**: Teams can work on separate slices independently
- **Simplified onboarding**: Developers can understand one slice at a time

## What is Vertical Slice Architecture?

In traditional layered architectures, code is organized by technical concerns (e.g., controllers, services, repositories). This often leads to high coupling between layers and makes it difficult to understand a single feature in isolation.

Vertical Slice Architecture takes a different approach, organizing code by business features or capabilities. Each "slice" contains all the components needed to implement a specific feature, from the user interface to the data access layer.

## Event Storming

CleanIAM's vertical slices were defined through an Event Storming process. Event Storming is a collaborative modeling technique that helps identify domain events, commands, and aggregates within a system.

The Event Storming process for CleanIAM revealed five main bounded contexts, which were translated into the following vertical slices:

1. **Identity**: Handling OAuth and OpenID Connect flows
2. **Applications**: Managing OIDC client applications
3. **Users**: Handling user management
4. **Scopes**: Managing OIDC scope definitions
5. **Tenants**: Managing multi-tenancy

Each bounded context operates with its own domain model and communicates with other contexts through well-defined events.

## CleanIAM Slice Structure

In CleanIAM, each slice follows a consistent internal structure based on Clean Architecture principles:

```
Slice/
├── Api/                  # API layer
│   ├── Controllers/      # API controllers
│   ├── Models/           # API models
│   └── Views/            # Views (for Identity slice)
├── Application/          # Application layer
│   ├── Commands/         # Commands and handlers
│   ├── EventHandlers/    # Domain event handlers
│   ├── Interfaces/       # Service interfaces
│   └── Queries/          # Queries and handlers
├── Core/                 # Core domain layer
│   ├── Events/           # Domain events
│   └── [Domain Models]   # Domain aggregates
└── Infrastructure/       # Infrastructure layer
    ├── AnticorruptionLayer/ # Event mapping
    └── Services/         # Service implementations
```

This structure ensures a clean separation of concerns while keeping related code together within each slice.

## Communication Between Slices

Slices communicate with each other primarily through events. When important state changes occur within a slice, it publishes domain events. Other slices can subscribe to these events and react accordingly.

This event-driven approach helps maintain loose coupling between slices, allowing them to evolve independently. The communication flow is facilitated by the WolverineFx messaging framework.

In the next section, we'll explore each individual slice in detail.
