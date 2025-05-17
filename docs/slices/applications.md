# Applications Slice

The Applications slice manages the registration and configuration of client applications that use CleanIAM for authentication and authorization. It handles the lifecycle of OAuth clients, including confidential and public clients.

## Responsibilities

The Applications slice handles:

- Registration of OAuth/OpenID Connect client applications
- Management of client credentials
- Configuration of client redirect URIs
- Client permissions and scopes
- Client application secret management

## Architecture

The Applications slice follows the standard CleanIAM architecture patterns. It interacts directly with OpenIddict's application registration system while providing a higher-level API and event-driven integration with other slices.

### Key Components

- **Controllers**: Handle CRUD operations for client applications
- **Commands**: Process application registration and management requests
- **Queries**: Retrieve application information
- **Events**: Signal important application lifecycle events
- **Domain Model**: Defines application-related entities

## Domain Events

The Applications slice defines and publishes the following domain events:

| Event                      | Description                                                     |
| -------------------------- | --------------------------------------------------------------- |
| `OpenIdApplicationCreated` | Triggered when a new client application is registered           |
| `OpenIdApplicationUpdated` | Triggered when a client application's configuration is modified |
| `OpenIdApplicationDeleted` | Triggered when a client application is removed                  |

## Commands

The Applications slice processes the following key commands:

- `CreateNewOpenIdApplicationCommand`: Registers a new client application
- `UpdateOpenIdApplicationCommand`: Updates an existing client application's configuration
- `DeleteOpenIdApplicationCommand`: Removes a client application

## Integration with OpenIddict

The Applications slice works directly with OpenIddict's application store. It manages entries in the `OpenIddictApplications` table, which contains information about all registered client applications.

Each application entry includes:

- Client ID
- Client secret (for confidential clients)
- SignIn Redirect URIs
- Post Logout Redirect URIs
- Allowed scopes
- Client type (confidential or public)
- Other client metadata

## Client Types

The Applications slice supports two types of clients as defined in the OAuth 2.0 specification:

### Confidential Clients

Confidential clients can securely store client secrets and typically run on a server. They use client authentication when requesting tokens.

Examples include:

- Web applications with server-side components
- Backend APIs
- Server daemons

### Public Clients

Public clients cannot securely store secrets and typically run in untrusted environments. They use PKCE (Proof Key for Code Exchange) for security.

Examples include:

- Single Page Applications (SPAs)
- Mobile applications
- Desktop applications

## Client Registration

The client registration process captures essential information:

1. Client name and description
2. Client type (confidential or public)
3. Redirect URIs
4. Allowed grant types
5. Allowed scopes
6. Token lifetimes
7. Consent requirements

For confidential clients, a secure client secret is generated during registration.

## User Interface

The Applications slice does not have its own frontend views but is managed through the React-based Management Portal. The portal provides an intuitive interface for:

- Viewing all registered applications
- Creating new applications
- Editing application settings
- Deleting applications

## API Endpoints

The Applications slice exposes RESTful API endpoints for:

- `GET /api/applications`: List all registered applications
- `GET /api/applications/{id}`: Get details of a specific application
- `POST /api/applications`: Create a new application
- `PUT /api/applications/{id}`: Update an existing application
- `DELETE /api/applications/{id}`: Delete an application

These endpoints are used by the Management Portal and can also be used by custom integration scripts or tools.

## Security Considerations

The Applications slice implements several security measures:

- Secure generation and storage of client secrets
- Validation of redirect URIs to prevent open redirector vulnerabilities
- Principle of least privilege for client permissions
- Audit logging of all application management operations
