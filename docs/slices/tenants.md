# Tenants Slice

The Tenants slice is responsible for managing multi-tenancy features in CleanIAM. It enables the system to serve multiple organizations or business units with isolated resources and configurations.

## Responsibilities

The Tenants slice handles:

- Tenant creation and management
- Updating tenant configurations
- Deleting tenants
- User-to-tenant assignments

## Architecture

The Tenants slice follows the standard CleanIAM slice structure with Clean Architecture principles. It defines a tenant domain model and provides integration with other slices through events.

### Key Components

- **Controllers**: Handle CRUD operations for tenant management
- **Commands**: Process tenant-related operations
- **Queries**: Retrieve tenant information
- **Events**: Signal important tenant lifecycle events
- **Domain Model**: Defines tenant-related entities

## Domain Events

The Tenants slice defines and publishes the following domain events:

| Event                  | Description                                         |
| ---------------------- | --------------------------------------------------- |
| `NewTenantCreated`     | Triggered when a new tenant is created              |
| `TenantUpdated`        | Triggered when a tenant's configuration is modified |
| `UserAssignedToTenant` | Triggered when a user is assigned to a tenant       |

## Commands

The Tenants slice processes the following key commands:

- `CreateTenantCommand`: Creates a new tenant
- `UpdateTenantCommand`: Updates a tenant's configuration
- `DeleteTenantCommand`: Removes a tenant
- `AssignUserToTenantCommand`: Assigns a user to a tenant

## Tenant Model

The Tenants slice defines a tenant model that includes:

- Tenant identifier
- Name and description

## Multi-Tenancy Implementation

CleanIAM supports multi-tenancy through:

### Tenant Isolation

- Data isolation between tenants (Using MartenDb conjoined tenancy setting)

### Tenant Identification

- Tenant identification through user claims (claim `tenant`)

### Tenant Management

- Centralized tenant administration
- Tenant provisioning and deprovisioning
- Tenant-level audit logs

## User Interface

The Tenants slice is managed through the React-based Management Portal, which provides:

- Tenant listing and search
- Tenant creation and configuration
- User assignment management

## API Endpoints

The Tenants slice exposes RESTful API endpoints for:

- `GET /api/tenants`: List all tenants
- `GET /api/tenants/{id}`: Get details of a specific tenant
- `POST /api/tenants`: Create a new tenant
- `PUT /api/tenants/{id}`: Update an existing tenant
- `DELETE /api/tenants/{id}`: Delete a tenant
- `GET /api/tenants/{id}/users`: List users in a tenant
- `POST /api/tenants/{id}/users`: Assign a user to a tenant
- `DELETE /api/tenants/{id}/users/{userId}`: Remove a user from a tenant

## Integration with Other Slices

The Tenants slice integrates with other slices to provide a complete multi-tenancy solution:

- **Identity Slice**: Tenant context in authentication and tokens
- **Users Slice**: User-to-tenant assignments
- **Applications Slice**: Tenant-specific client applications
- **Scopes Slice**: Tenant-specific scope definitions

## Security Considerations

The Tenants slice implements several security measures:

- Strong data isolation between tenants
- Tenant-specific access controls
- Validation of tenant ownership
- Prevention of cross-tenant access
- Audit logging of all tenant management operations
