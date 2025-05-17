# Getting Started with CleanIAM

CleanIAM is a modern, open-source Identity and Access Management (IAM) solution specifically designed for the .NET ecosystem. It's built to replace deprecated solutions like IdentityServer4 and provides a comprehensive, customizable, and developer-friendly IAM platform.

## Prerequisites

- .NET 8.0 SDK or later
- PostgreSQL database server
- Docker (optional, for containerized deployment)

## Quick Installation of base CleanIAM

### Clone the Repository

```bash
git clone https://github.com/CleanIAM/CleanIAM.git
cd CleanIAM
```

### Configure Environment Variables

Create a `.env` file based on the provided example:

```bash
cp src/CleanIAM.Identity/.env.example .env
```

- Update the configuration with your database connection string and other settings.
- Update the appsettings in `src/CleanIAM.Host/appsettings.json` to match your environment.

### Run Migrations

- You only need to run the migrations if you updated the OpenIddict schema, since Marten doesn't have to run migrations.

#### Apply a migration

```bash
cd src/CleanIAM.SharedKernel
dotnet ef database update
```

#### Create a migration

- If you update datable schema you can create a migration using the following command:

```bash
cd src/CleanIAM.SharedKernel
dotnet ef migrations add <migration_name>
```

### Seed the database with initial data:

- To seed the database with initial data, run the following command:

```bash
cd src/CleanIAM.DbSeed
dotnet restore
dotnet run
```

### Run Locally

To run the application locally:

```bash
cd src/CleanIAM.Host
dotnet restore
dotnet run
```

## Accessing the Application

- The Identity server is available at: `http://localhost:5000`
- The Management Portal FE is available at: `http://localhost:3001` (when running in development mode)

## Initial Setup

When you run CleanIAM seeding, it will create a default master administrator account:

- Username: `master@admin.com`
- Password: `Asdfasdf1.`

**Important**: You should change this password as soon as possible.

## Next Steps

After installation, you might want to:

1. [Configure your first OIDC application](/slices/applications)
2. [Set up external identity providers](/slices/identity)
3. [Configure multi-factor authentication](/slices/users)
4. [Learn about the architecture](/architecture)
