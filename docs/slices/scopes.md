# Scopes Slice

The Scopes slice is responsible for managing OAuth 2.0 scopes in CleanIAM. Scopes are fundamental to the OAuth authorization model, defining the permissions and access levels that client applications can request.

## Responsibilities

The Scopes slice handles:

- Definition and management of OAuth scopes
- Scope descriptions and documentation
- Default scope configurations

## Architecture

The Scopes slice follows the standard CleanIAM slice structure with Clean Architecture principles. It interacts directly with OpenIddict's scope management system.

### Key Components

- **Controllers**: Handle CRUD operations for OAuth scopes
- **Commands**: Process scope management requests
- **Queries**: Retrieve scope information
- **Events**: Signal important scope lifecycle events
- **Domain Model**: Defines scope-related entities

## Domain Events

The Scopes slice defines and publishes the following domain events:

| Event          | Description                                               |
| -------------- | --------------------------------------------------------- |
| `ScopeCreated` | Triggered when a new OAuth scope is defined               |
| `ScopeUpdated` | Triggered when an existing scope's definition is modified |
| `ScopeDeleted` | Triggered when a scope is removed                         |

## Commands

The Scopes slice processes the following key commands:

- `CreateScopeCommand`: Defines a new OAuth scope
- `UpdateScopeCommand`: Modifies an existing scope's definition
- `DeleteScopeCommand`: Removes a scope definition

## Integration with OpenIddict

The Scopes slice works directly with OpenIddict's scope store. It manages entries in the `OpenIddictScopes` table, which contains information about all defined OAuth scopes.

Each scope entry includes:

- Scope name
- Display name
- Description
- Resources allowed by the scope
- Other scope metadata

## Default Scopes

CleanIAM includes support for standard OpenID Connect scopes:
Roles,

- `openid`: Indicates an OpenID Connect request
- `profile`: Requests access to the user's basic profile information
- `email`: Requests access to the user's email address
- `roles`: Requests access to the user's roles
- `offline_access`: Requests a refresh token

Additionally, the Scopes slice allows defining custom scopes for specific application needs.

- default scopes are generated as part of the seeding process

## Scope Management

The scope management process allows administrators to:

1. Create new scopes with appropriate descriptions
2. Define which resources are allowed by the scope
3. Update existing scopes
4. Delete scopes that are no longer needed

## User Interface

The Scopes slice is managed through the React-based Management Portal, which provides:

- Listing of all defined scopes
- Creation of new scope definitions
- Editing of existing scopes
- Deletion of scopes

## API Endpoints

The Scopes slice exposes RESTful API endpoints for:

- `GET /api/scopes`: List all defined scopes
- `GET /api/scopes/default`: List default scopes
- `POST /api/scopes`: Create a new scope definition
- `PUT /api/scopes/{id}`: Update an existing scope
- `DELETE /api/scopes/{id}`: Delete a scope

These endpoints are used by the Management Portal and can also be used for custom integrations.

## Security Considerations

The Scopes slice plays a crucial role in CleanIAM's security model:

- It enforces the principle of least privilege by allowing fine-grained access control
- It ensures that client applications can only request appropriate permissions
- It provides transparency to users about what data is being shared
- It maintains audit logs of scope definition changes
