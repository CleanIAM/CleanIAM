# Frontend Overview

CleanIAM provides a comprehensive frontend experience for both users and administrators. The frontend components are designed with modern UI principles, focusing on usability, accessibility, and responsive design.

## Frontend Architecture

CleanIAM employs two distinct frontend approaches:

1. **Server-side MVC Views**: Used for authentication flows and user-facing interfaces
2. **React Management Portal**: Used for administration and management interfaces

This dual approach provides the best of both worlds:

- Server-side rendering for security-critical authentication flows
- Rich, interactive experience for administrative interfaces

## Technology Stack

The frontend stack includes:

### Authentication UI (MVC)

- ASP.NET Core MVC
- Razor views
- Tailwind CSS for styling
- Minimal JavaScript for enhanced interactions

### Management Portal (React)

- React for component-based UI
- TypeScript for type safety
- TanStack Query (React Query) for data fetching
- Custom API client generated from OpenAPI schema
- Modern UI component library
- Responsive design principles

## Design Principles

CleanIAM's frontend design adheres to the following principles:

- **Simplicity**: Clean, straightforward interfaces that focus on the task at hand
- **Accessibility**: Compliance with WCAG guidelines for inclusive user experience
- **Responsiveness**: Adapting to different screen sizes and devices
- **Consistency**: Uniform design language across all interfaces
- **Feedback**: Clear communication of system status and user actions
- **Performance**: Fast loading and responsive interactions

## Frontend Components

The frontend is organized into several key components:

### Authentication Components

- Login page
- Registration page
- Multi-factor authentication flow
- Password reset flow
- Email verification
- Account selection
- OAuth consent screen

### Administration Components

- Dashboard
- User management
- Application management
- Scope management
- Tenant management
- Profile overview (for all users, includes MFA management)

In the following sections, we'll explore the MVC views for authentication and the React Management Portal in more detail.
