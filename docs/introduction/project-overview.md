# Project Overview

CleanIAM is an open-source Identity and Access Management (IAM) solution built for the .NET ecosystem. It addresses the gap left by the deprecation of IdentityServer4, providing a modern, extensible, and developer-friendly alternative.

## Key Features

- **OAuth 2.0 and OpenID Connect Compliance**: Full implementation of OAuth 2.0 and OpenID Connect protocols, including Authorization Code flow with PKCE and Client Credentials flow
- **User Management**: Comprehensive user management with registration, profile editing, and password reset
- **Multi-Factor Authentication**: Support for Time-based One-Time Password (TOTP) authentication
- **External Identity Providers**: Integration with external identity providers like Google and Microsoft
- **Role-Based Authorization**: Built-in role management for application authorization
- **Client Application Management**: Register and manage client applications that connect to CleanIAM
- **Customizable UI**: Modern, responsive UI for authentication flows and a React-based management portal
- **Event-Driven Architecture**: Events-based communication between system components for loose coupling and extensibility
- **Vertical Slice Architecture**: Organized by domain features rather than technical layers for improved maintainability
- **Docker Support**: Easy deployment using containerization

## Architecture Principles

CleanIAM is built with the following architectural principles:

1. **Modularity**: The system is divided into vertical slices based on domain features
2. **Clean Architecture**: Each slice follows clean architecture principles
3. **Event-Driven Design**: Components communicate via events
4. **Extensibility**: Designed to be easily extended and customized
5. **Developer Experience**: Focus on making the system easy to understand and modify

## Project Structure

The project is structured into several key components:

- **CleanIAM.Host**: The main application entry point
- **CleanIAM.Identity**: Handles authentication and identity management
- **CleanIAM.Users**: Manages user accounts and profiles
- **CleanIAM.Applications**: Manages client applications
- **CleanIAM.Scopes**: Handles OAuth scopes
- **CleanIAM.Tenants**: Manages multi-tenancy
- **CleanIAM.SharedKernel**: Contains shared components
- **CleanIAM.Events**: Defines domain events
- **CleanIAM.ManagementPortal**: React-based admin interface

Each of these components is designed as a vertical slice, containing all necessary layers (API, Application, Domain, Infrastructure) for its specific domain functionality.

## Target Audience

CleanIAM is designed for:

- **.NET Developers**: Who need a modern IAM solution that integrates well with the .NET ecosystem
- **Organizations**: Looking for a customizable and extensible IAM system
- **Former IdentityServer4 Users**: Seeking a suitable replacement

In the following sections, we'll dive deeper into each aspect of the CleanIAM architecture and implementation.
