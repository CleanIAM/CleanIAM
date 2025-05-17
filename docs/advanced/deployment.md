# Deployment

This section covers deployment options and best practices for CleanIAM in various environments.

## Deployment Options

CleanIAM supports multiple deployment models to fit different organizational needs:

### Docker Deployment

The simplest way to deploy CleanIAM is using Docker:

- Build the Docker image form source code

```bash
cd CleanIAM
docker build -t cleaniam .
```

### Docker Compose

For a complete setup including the database, use Docker Compose:

```yaml
version: "3.8"

services:
  cleaniam:
    image: cleaniam:latest
    ports:
      - "5000:5000"
    environment: -ENVIRONMENT=VARIABLES
    depends_on:
      - postgres
    restart: unless-stopped

  postgres:
    image: postgres:14
    environment:
      - POSTGRES_PASSWORD=password
      - POSTGRES_USER=postgres
      - POSTGRES_DB=postgres
    restart: unless-stopped
```

### Azure App Service

CleanIAM can be deployed to Azure App Service:

1. Create an Azure App Service plan
2. Create a new Web App
3. Configure deployment source (GitHub, Docker Hub, etc.)
4. Set environment variables in Configuration
5. Configure custom domain and SSL if needed

## Configuration

### Environment Variables

CleanIAM can be configured through environment variables:

| Variable | Description |
| --- | --- |
| **Database Settings** | |
| `DbSettings__DatabaseNames__oidc` | OIDC database name |
| `DbSettings__DatabaseNames__MartenDb` | Marten database name |
| `DbSettings__ConnectionStrings__oidc` | OIDC database connection string |
| `DbSettings__ConnectionStrings__MartenDb` | Marten database connection string |
| **Authentication Settings** | |
| `OpenIddict__EncryptionKey` | OpenIddict encryption key |
| `Authentication__OpenIddict__ExternalProviders__Google__ClientId` | Google OAuth client ID |
| `Authentication__OpenIddict__ExternalProviders__Google__ClientSecret` | Google OAuth client secret |
| `Authentication__OpenIddict__ExternalProviders__Microsoft__ClientId` | Microsoft OAuth client ID |
| `Authentication__OpenIddict__ExternalProviders__Microsoft__ClientSecret` | Microsoft OAuth client secret |
| **Email Settings** | |
| `Coravel__Mail__Driver` | Mail driver |
| `Coravel__Mail__Host` | SMTP host |
| `Coravel__Mail__Port` | SMTP port |
| `Coravel__Mail__Username` | SMTP username |
| `Coravel__Mail__Password` | SMTP password |
| `Coravel__Mail__From__Address` | Sender email address |
| `Coravel__Mail__From__Name` | Sender name |
| **URL and Routing Settings** | |
| `HttpRoutes__IdentityBaseUrl` | Identity service base URL |
| `HttpRoutes__ManagementPortalBaseUrl` | Management portal base URL |
| `UrlShortener__BaseUrl` | URL shortener service URL |
