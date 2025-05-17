# MVC Views

The authentication UI in CleanIAM is built using ASP.NET Core MVC with Razor views. This approach was chosen for security-critical authentication flows to ensure maximum security by keeping sensitive logic server-side.

## Technology Stack

The MVC views use:

- ASP.NET Core MVC framework
- Razor view engine
- Tailwind CSS for styling
- Minimal JavaScript for enhanced interactions
- HTML5 semantic elements for accessibility

## View Structure

The MVC views are organized into logical sections:

- **Auth**: OAuth consent and authorization views
- **EmailVerification**: Email verification workflow
- **Invitation**: User invitation acceptance flows
- **Mfa**: Multi-factor authentication setup and validation
- **PasswordReset**: Password reset workflow
- **Shared**: Layouts and partial views
- **Signin**: Login screens and flows
- **Signup**: Registration screens and flows

## Authentication Flows

The MVC views implement several key authentication flows:

### Sign In Flow

1. User enters credentials (username/email and password)
2. System validates credentials
3. If MFA is enabled, user is prompted for the second factor
4. User is authenticated and redirected to the original application

### Registration Flow

1. User provides registration details (email, password, profile info)
2. System validates input and creates the account
3. Verification email is sent
4. User verifies email address
5. Account is activated

### Password Reset Flow

1. User requests password reset
2. System sends reset email with secure token
3. User clicks link and enters new password
4. Password is updated

### MFA Setup Flow

1. User initiates MFA setup
2. System generates TOTP secret and QR code
3. User scans QR code with authenticator app
4. User confirms setup by entering a TOTP code
5. System enables MFA for the account

## Design Implementation

The MVC views implement a clean, minimalist design focused on the task at hand. Key design features include:

- **Consistent Layout**: All views share a common layout for brand consistency
- **Responsive Design**: Views adapt to different screen sizes using Tailwind's responsive utilities
- **Error Handling**: Clear, user-friendly error messages
- **Progressive Enhancement**: Core functionality works without JavaScript, with enhancements when available

## Customization Options

The MVC views can be customized through:

- **Theme Customization**: Tailwind configuration allows easy color scheme updates
- **Layout Adjustments**: Modified shared layouts
- **Content Changes**: Update text, labels, and messaging
- **Branding**: Custom logos and brand elements
- **Additional Fields**: Extend forms with custom fields

## Integration with Identity Slice

The MVC views are tightly integrated with the Identity slice:

- Views are rendered by controllers in the Identity slice
- View models are defined in the Identity slice
- Form submissions are processed by Identity slice endpoints
- Authentication logic is handled by Identity slice services

## Security Considerations

The MVC views implement several security best practices:

- **CSRF Protection**: Anti-forgery tokens in all forms
- **Input Validation**: Both client and server-side validation
- **Secure Redirects**: Validation of all redirect URLs

## Example Views

### Login View

The login view provides a simple form for username and password entry, with options for social login and password reset.

![Login View](/Figures/SignIn_screen.png)

### MFA Setup View

The MFA setup view displays a QR code for scanning with an authenticator app, along with manual entry options and clear instructions.

![MFA Setup View](/Figures/MFA_config_screen.png)

### Consent Screen

The OAuth consent screen clearly displays the requesting application and the permissions it's requesting, allowing the user to make an informed decision.

![Consent Screen](/Figures/account_chooser_screen.png)
