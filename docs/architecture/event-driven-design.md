# Event-Driven Design

CleanIAM implements an event-driven architecture to facilitate communication between different parts of the system while maintaining loose coupling. This approach enhances scalability, maintainability, and extensibility.

## Core Concepts

In CleanIAM's event-driven design:

- **Domain Events**: Represent significant state changes or business occurrences
- **Commands**: Represent requests to perform actions that may change system state
- **Queries**: Represent requests for information that don't change state
- **Event Handlers**: Components that react to events
- **Command Handlers**: Components that process commands
- **Query Handlers**: Components that process queries

## Benefits of Event-Driven Architecture

The event-driven approach offers several advantages:

1. **Loose Coupling**: Slices don't need direct references to each other
2. **Extensibility**: New functionality can be added by subscribing to existing events
3. **Auditability**: Events provide a natural audit trail of system activity
4. **Scalability**: Event-based communication can scale horizontally
5. **Resilience**: Failures in one component don't necessarily cascade to others

## Implementation with WolverineFx

CleanIAM uses the [WolverineFx](https://wolverinefx.net/) framework for message handling. WolverineFx provides:

- In-process message bus for events, commands, and queries
- Automatic handler discovery and routing
- Support for CQRS pattern implementation
- Middleware and message pipelines
- Asynchronous message handling

## Event Flow

The typical flow of events in CleanIAM follows this pattern:

1. A command is dispatched from a controller or another handler
2. The command handler validates the command and performs business logic
3. If successful, the handler publishes one or more domain events
4. Event handlers in various slices react to the events
5. Event handlers may dispatch additional commands or queries

This pattern keeps business logic encapsulated and allows for clear separation of concerns.

## Code Structure

The event-driven design is reflected in CleanIAM's code structure:

- Domain events are defined in each slice's `Core/Events` directory
- Commands and their handlers are in each slice's `Application/Commands` directory
- Event handlers are in each slice's `Application/EventHandlers` directory
- The `AnticorruptionLayer` in each slice maps between local and global events

In the next sections, we'll explore each vertical slice in detail, including their specific events and commands.
