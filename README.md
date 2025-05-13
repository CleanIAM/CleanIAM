# masters-thesis

This is my Masters thesis 2025

## Documentation is available at [docs](https://cleaniam.github.io/CleanIAM/)

# This repo is a work in progress and is not yet complete!!!

## Prerequisites

- **Tailwindcss** This project uses Tailwindcss for styling and requires Tailwind CLI for build. You can install it by
  running `npm install tailwindcss @tailwindcss/cli`.

## Database

- This project uses postgres database underneath.

### Migrations

#### Create a migration

- In the root of the 'CleanIAM.SharedKernel' project invoke following command

```bash
  dotnet ef migrations add <migration_name>
```

#### Apply a migration

- In the root of the 'CleanIAM.SharedKernel' project invoke following command

```bash
  dotnet ef database update
```