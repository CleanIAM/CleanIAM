# Architecture Overview

CleanIAM employs a thoughtfully designed architecture that prioritizes modularity, maintainability, and extensibility. This section provides a high-level overview of the architectural design principles that guide the CleanIAM project.

## Architectural Principles

CleanIAM is built on several key architectural patterns:

1. **Vertical Slice Architecture**: The system is organized by features or business capabilities rather than technical layers
2. **Clean Architecture**: Each slice maintains a clear separation of concerns with domain logic at the core
3. **Event-Driven Design**: Components communicate through events, minimizing direct dependencies
4. **Modular Monolith**: The initial implementation is a modular monolith that can be evolved into microservices if needed

## High-Level Architecture

At a high level, CleanIAM consists of:

- **Core Domain Model**: Represents the business entities and logic
- **Vertical Slices**: Feature-specific modules that encapsulate all related functionality
- **Event System**: Enables communication between slices
- **External Interfaces**: APIs and UIs for interacting with the system

## System Components

The main components of CleanIAM include:

### Backend Components

- **Identity Service**: Handles authentication, OAuth flows, and token management
- **User Management**: Manages user accounts, profiles, and credentials
- **Application Management**: Registers and configures client applications
- **Scope Management**: Defines and manages OAuth scopes
- **Tenant Management**: Handles multi-tenancy features

### Frontend Components

- **Authentication UI**: MVC-based views for login, registration, and account management
- **Management Portal**: React-based administrative interface

### Data Storage

- **PostgreSQL Database**: Primary data store
- **Entity Framework Core**: Used by OpenIddict for data access
- **MartenDb**: Document database for domain models and event storage

## Technology Stack

CleanIAM leverages modern technologies:

- **.NET 8**: For the core backend services
- **OpenIddict**: For OAuth 2.0 and OpenID Connect implementation
- **MartenDb**: For document database and event sourcing
- **WolverineFx**: For message handling and in-process messaging
- **React**: For the management portal frontend
- **Docker**: For containerization and deployment

## Communication Patterns

CleanIAM uses:

- **Commands**: For requests that change state
- **Queries**: For data retrieval
- **Events**: For notifying components about state changes
- **HTTP APIs**: For external communication

In the following sections, we'll dive deeper into each aspect of the architecture, starting with the vertical slice approach.
