# MyApp – Coworkee reference app

A Blazor (WASM + BFF) and ASP.NET Core app built on the **Coworkee** packages. It is the starting point for new apps and shows how a module plugs into the platform.

## What Coworkee brings

Sign-in through its own auth server (OpenIddict, passkeys, 2FA), setup wizard on an empty database, users, groups, roles, permission matrix and resource permissions, settings, mail templates, audit log, themes, realtime updates, notifications, background jobs (Hangfire), blob storage (file system, S3, Azure Blob) and the admin UI. Everything runs under .NET Aspire with PostgreSQL, Redis and Mailpit.

## What the app adds (examples to copy or delete)

| Module | Shows |
| --- | --- |
| `MyApp.Catalog` | Brands and products: entities with `IModelContributor`, permissions (`Catalog.View`, `Catalog.Manage`), commands and queries on the Coworkee dispatcher, minimal API endpoints, paging and search, live updates via `[Realtime]`, a dashboard query |
| `MyApp.Documents` | Files on Coworkee.Storage: typed documents, public or private, upload, download, preview in **MudExFileDisplay** for safe types (served with `nosniff` and a sandbox CSP) |

Pages: dashboard (`/`), brands, products, documents, document types. The admin area (users, roles, settings, mail, themes, audit, jobs) comes from `Coworkee.Client.Blazor`.

## Getting started

1. .NET 10 SDK, Docker.
2. Coworkee packages: clone `CoworkeeLib` next to this repository and run `pwsh build/pack-local.ps1` there (fills `../coworkee/artifacts/nuget`, which `nuget.config` points at).
3. `dotnet run --project src/MyApp.AppHost`, open the web app from the Aspire dashboard and complete the setup wizard (the setup token is in the api log, or set `MyApp:SetupToken` for the AppHost).

## Your own app

`pwsh ./rename.ps1 -new Contoso` renames projects, namespaces and files (`MyApp` → `Contoso`, `myapp` → `contoso`). Then add a module next to `MyApp.Catalog`: a `CoworkeeModule` with `IWebModule`, register it in `MyAppDatabaseModule`, add a migration:

```
dotnet ef migrations add <Name> --project src/MyApp.Infrastructure --startup-project src/MyApp.Infrastructure --output-dir Migrations
```

## Structure

| Project | Role |
| --- | --- |
| `MyApp.AppHost` | Aspire topology (database, redis, mail, auth, api, web, migrations) |
| `MyApp.Auth` | Auth server host (Coworkee.AuthServer) |
| `MyApp.Api` | API host: Coworkee modules plus the app's modules |
| `MyApp.Web` / `MyApp.Web.Client` | BFF host and Blazor WASM client |
| `MyApp.Infrastructure` | DbContext, migrations, module composition |
| `MyApp.Migrations` | Applies migrations before the hosts start |
| `MyApp.Contracts` | DTOs and permission names shared with the client |

Tests: API tests on Testcontainers (`tests/MyApp.Api.Tests`), bUnit (`tests/MyApp.Web.Client.Tests`), migration and Aspire start-up tests. CI needs the secret `COWORKEE_REPO_TOKEN` to clone CoworkeeLib.

The former MediatR-based template lives in the git history before the `feat/coworkee-migration` branch.
