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

| Variable                 | Description         | Default       |
| ------------------------ | ------------------- | ------------- |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |
