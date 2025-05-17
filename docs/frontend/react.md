# React Management Portal

The CleanIAM Management Portal is a modern, React-based administrative interface for managing all aspects of the CleanIAM system. It provides a rich, interactive experience for administrators and users.

## Technology Stack

The Management Portal uses:

- **React**: For component-based UI development
- **TypeScript**: For type safety and improved developer experience
- **TanStack Query (React Query)**: For data fetching and state management
- **React Router**: For client-side routing
- **shadcn/ui**: For component styling and UI framework
- **Tailwind CSS**: For utility-first styling
- **Axios**: For HTTP requests
- **Orval**: For generating API clients from OpenAPI schema

## Architecture

The Management Portal follows modern React application architecture:

- **Component-based structure**: Reusable UI components
- **Page-based organization**: Separate components for each route
- **Custom hooks**: For shared logic and state management
- **Context providers**: For global state and configuration
- **TypeScript interfaces**: For API contract and type safety
- **Generated API clients**: For type-safe API communication

## Key Features

The Management Portal provides interfaces for managing:

- **Users**: Create, update, delete, and manage user accounts
- **Applications**: Register and configure OIDC client applications
- **Scopes**: Define and manage OAuth scopes
- **Tenants**: Create and configure tenant environments
- **User Profiles**: Self-service user profile management
- **MFA**: Multi-factor authentication setup and management

## Pages and Modules

The portal includes the following main sections:

- **Dashboard**: Overview of system statistics and status
- **Users**: User management interface
- **Applications**: Client application management
- **Scopes**: OAuth scope definitions
- **Tenants**: Tenant management (for multi-tenant deployments)
- **Profile**: User profile self-management
- **Settings**: System configuration

## API Integration

The Management Portal interacts with CleanIAM's backend through a RESTful API. The API client is automatically generated from the OpenAPI schema provided by the backend, ensuring type safety and up-to-date API contracts.

### API Client Generation

The API client generation process:

1. Backend generates OpenAPI schema
2. Orval uses schema to generate TypeScript clients
3. Generated clients include types, queries, and mutations
4. The portal uses these clients for all API communication

This approach ensures that the frontend and backend stay in sync, with TypeScript providing compile-time validation of API contracts.

## State Management

The portal uses a combination of:

- **TanStack Query**: For server state management (API data)
- **React Context**: For global application state
- **React useState/useReducer**: For component-local state

This approach provides a good balance between simplicity and scalability.

## Authentication

The Management Portal authenticates with CleanIAM using OAuth 2.0 / OpenID Connect:

1. User navigates to the portal
2. Portal redirects to CleanIAM login
3. User authenticates with CleanIAM
4. CleanIAM redirects back with authorization code
5. Portal exchanges code for tokens
6. Portal uses access token for API requests

## Authorization

The portal implements role-based access control:

- Different user roles see different sections of the portal
- UI elements are conditionally rendered based on permissions
- API requests are authorized with appropriate scopes
- Unauthorized actions are prevented at the UI level

## User Experience

The Management Portal provides a modern, intuitive user experience:

- **Responsive design**: Works on desktops, tablets, and mobile devices
- **Consistent UI**: Uniform design language across all interfaces
- **Immediate feedback**: Real-time validation and feedback on user actions
- **Error handling**: User-friendly error messages and recovery options

## Customization

The Management Portal can be customized through:

- **Theme configuration**: Customize colors, typography, and spacing
- **Component overrides**: Replace or modify individual components
- **Layout adjustments**: Customize page layouts and navigation

## Deployment

The Management Portal can be deployed in several ways:

- **Standalone**: Deployed as a static site on any web server
- **Containerized**: Deployed as a Docker container

## Developer Experience

The portal prioritizes developer experience:

- **Hot reloading**: Instant feedback during development
- **Type safety**: TypeScript catches errors at compile time
- **Generated API clients**: Type-safe API communication
- **Component library**: Ready-to-use UI components
- **Consistent patterns**: Predictable code organization
