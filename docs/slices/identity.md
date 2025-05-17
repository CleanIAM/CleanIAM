# Identity Slice

The Identity slice is responsible for handling authentication and OAuth/OpenID Connect flows in CleanIAM. It's one of the core components of the system, managing user authentication, token issuance, and OAuth protocol implementation.

## Responsibilities

The Identity slice handles:

- OAuth 2.0 and OpenID Connect protocol flows
- User authentication and session management
- Token issuance and validation
- Email verification
- Password reset
- User invitations
- Login UI and flows

## Architecture

The Identity slice follows the standard CleanIAM slice structure with Clean Architecture principles. It's built on top of the OpenIddict framework, which provides the core OAuth and OpenID Connect functionality.

### Key Components

- **Controllers**: Handle OAuth endpoints and authentication flows
- **Views**: Provide the user interface for authentication
- **Commands**: Process authentication requests and manage user credentials
- **Events**: Signal important authentication-related occurrences
- **Domain Model**: Defines authentication-related entities

## Domain Events

The Identity slice defines and publishes the following domain events:

| Event                          | Description                                              |
| ------------------------------ | -------------------------------------------------------- |
| `EmailVerificationRequestSent` | Triggered when a verification email is sent to a user    |
| `NewUserSignedUp`              | Triggered when a new user completes registration         |
| `PasswordResetRequestSent`     | Triggered when a password reset email is sent            |
| `PasswordReset`                | Triggered when a user successfully resets their password |
| `UserAccountSetup`             | Triggered when a user completes initial account setup    |
| `UserEmailVerified`            | Triggered when a user verifies their email address       |
| `UserInvitationCreated`        | Triggered when an invitation is created for a new user   |
| `UserLoggedIn`                 | Triggered when a user successfully logs in               |

## Commands

The Identity slice processes the following key commands:

### User Management

- `CreateNewUserCommand`: Creates a new user account

### Email Verification

- `SendEmailVerificationRequestCommand`: Sends an email verification link
- `VerifyEmailCommand`: Processes an email verification token

### Password Management

- `SendPasswordResetRequestCommand`: Initiates the password reset process
- `ResetPasswordCommand`: Processes a password reset

### Invitations

- `CreateUserInvitationCommand`: Creates a new user invitation
- `SetupUserAccountCommand`: Processes an invitation acceptance

## Integration with OpenIddict

The Identity slice integrates with OpenIddict to implement OAuth 2.0 and OpenID Connect protocols. OpenIddict handles:

- Authorization code flow with PKCE
- Client credentials flow
- Refresh token flow
- ID token generation
- Access token validation
- Token introspection

## User Interface

The Identity slice includes the MVC views for:

- Login page
- Registration page
- Multi-factor authentication
- Password reset
- Email verification
- Account selection
- Consent screen for OAuth authorization

The UI is designed to be simple, responsive, and user-friendly, with a focus on security and usability.

## External Identity Providers

The Identity slice supports authentication through external identity providers like:

- Google
- Microsoft
- Other OpenID Connect compatible providers

This functionality allows users to sign in using their existing accounts with these providers.

## Security Considerations

The Identity slice implements several security best practices:

- Password hashing using Argon2id
- Secure token generation and validation
- Protection against common authentication attacks
- Secure cookie handling
- CSRF protection
