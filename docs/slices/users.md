# Users Slice

The Users slice is responsible for managing user accounts, profiles, and authentication settings. It handles user data management, multi-factor authentication (MFA), and user-related operations.

## Responsibilities

The Users slice handles:

- User profile management
- User account status (enabled/disabled)
- User roles and permissions
- Multi-factor authentication (MFA) configuration
- User account deletion
- User invitation processing
- Profile data storage

## Architecture

The Users slice follows the standard CleanIAM slice structure with Clean Architecture principles. It maintains its own user domain model while integrating with the Identity slice for authentication.

### Key Components

- **Controllers**: Handle user management API endpoints
- **Commands**: Process user profile operations
- **Queries**: Retrieve user information
- **Events**: Signal important user account events
- **Domain Model**: Defines user-related entities

## Domain Events

The Users slice defines and publishes the following domain events:

### User Account Events

| Event          | Description                                             |
| -------------- | ------------------------------------------------------- |
| `UserUpdated`  | Triggered when a user's profile information is modified |
| `UserDeleted`  | Triggered when a user account is deleted                |
| `UserDisabled` | Triggered when a user account is disabled               |
| `UserEnabled`  | Triggered when a user account is enabled                |
| `UserInvited`  | Triggered when a user is invited to create an account   |

### Multi-Factor Authentication Events

| Event                  | Description                                       |
| ---------------------- | ------------------------------------------------- |
| `MfaConfiguredForUser` | Triggered when MFA is initially set up for a user |
| `MfaEnabledForUser`    | Triggered when MFA is activated for a user        |
| `MfaDisabledForUser`   | Triggered when MFA is deactivated for a user      |

## Commands

The Users slice processes the following key commands:

### User Management

- `UpdateUserCommand`: Updates a user's profile information
- `DeleteUserCommand`: Permanently removes a user account
- `DisableUserCommand`: Temporarily disables a user account
- `EnableUserCommand`: Re-enables a previously disabled user account
- `InviteUserCommand`: Creates an invitation for a new user

### Multi-Factor Authentication

- `ConfigureMfaCommand`: Sets up MFA for a user
- `EnableMfaCommand`: Activates MFA for a user
- `DisableMfaCommand`: Deactivates MFA for a user
- `ValidateMfaCommand`: Verifies an MFA code (Part of the MFA setup flow)

## User Model

The Users slice defines a comprehensive user model that includes:

- Basic profile information (name, email, etc.)
- Authentication settings
- MFA configuration
- Account status
- Roles and permissions
- Audit information (creation date, etc.)

## Multi-Factor Authentication

The Users slice implements Time-based One-Time Password (TOTP) as the primary MFA method. The MFA implementation:

1. Generates a secure secret key for each user
2. Provides QR codes for easy setup with authenticator apps
3. Validates TOTP codes during authentication
4. Can be enabled/disabled per user

## User Interface

The Users slice is primarily managed through the React-based Management Portal, which provides:

- User listing and search
- User profile editing
- Role assignment
- Account status management
- MFA management

Users can also manage their own profiles and MFA settings through dedicated sections in the portal.

## API Endpoints

The Users slice exposes RESTful API endpoints for:

User management:

- `GET /api/users`: List all users
- `GET /api/users/{id}`: Get details of a specific user
- `POST /api/users`: Create a new user
- `PUT /api/users/{id}`: Update an existing user
- `DELETE /api/users/{id}`: Delete a user
- `PUT /api/users/{id}/disabled`: Disable a user
- `PUT /api/users/{id}/enabled`: Enable a user
- `POST /api/users/invited`: Invite a new user
- `POST /api/users/invitation/email`: Resend invitation email
- `POST /api/users/{id}/mfa/configure`: Configure MFA
- `Delete /api/users/{id}/mfa/enabled`: Disable MFA for specific user

User profile management:

- `GET /api/user`: Get the current user's profile
- `PUT /api/user`: Update the current user's profile
- `PUT /api/user/mfa/enabled`: Enable or disable MFA for the current user
- `POST /api/user/mfa/validate`: Validate MFA code for the current user
- `GET mfa/configuration`: Generate QR code for MFA setup
- `POST mfa/configuration`: Validate MFA code for setup
- `DELETE mfa/configuration`: Clear MFA configuration and disable MFA for the current user

## Integration with Identity Slice

The Users slice works closely with the Identity slice:

- Each slice has its own domain model representing user, these models are synchronized through events.

## Security Considerations

The Users slice implements several security measures:

- Role-based access control for user management
- Validation of all user inputs
- Audit logging of user management operations
