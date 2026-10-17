# AGENTS.md

Rules for coding agents in this app, the Coworkee reference app (see `README.md`). It is built on Coworkee (.NET 10, Aspire, Blazor WebAssembly, EF Core); docs: https://fgilde.github.io/CoworkeeLib/

## Layout

| Path | What goes there |
|---|---|
| `src/MyApp.Contracts` | DTOs, requests, permission names (`<Module>Permissions`), texts (`Localization/<culture>.json`) |
| `src/MyApp.<Module>` | a feature module: `Domain/` entities, `Features/<Plural>/Commands/<UseCase>/` and `Queries/<UseCase>/` request, handler, validator, `Permissions/` definitions, `Persistence/` table mapping, `Endpoints/`, `<Module>Module.cs` wiring |
| `src/MyApp.Application` | requests that belong to no module |
| `src/MyApp.Infrastructure` | DbContext, migrations, module composition (`MyAppDatabaseModule` depends on every module) |
| `src/MyApp.Migrations` | applies migrations and the seed (`DemoSeed.cs`: roles and their permissions, `DemoUsers.cs`: the users of `LOGIN.md`, `Demo*.cs`: sample data) |
| `src/MyApp.Web.Client` | Blazor pages (`Pages/<Module>/`), API clients (`Api/`), navigation (`Navigation/MyAppNavigation.cs`), registrations in `Program.cs` |
| `src/MyApp.Api`, `src/MyApp.Web`, `src/MyApp.Auth` | hosts: API, web app with backend for frontend, auth server; rarely touched |
| `src/MyApp.AppHost` | Aspire topology |
| `src/MyApp.AirWorks` | AirWorks web desktop (TypeScript in `ClientApp`, `@coworkee/client` and `@coworkee/airworks` from npm) |
| `src/MyApp.ElsaStudio` | Elsa Studio, the workflow designer |
| `src/MyApp.Client` | typed .NET client of the API |
| `sdk/` | OpenAPI snapshot (`openapi.json`) and the generated TypeScript client (`typescript`) |
| `tests/` | API tests against a real database (`MyApp.Api.Tests`), migration tests, app host tests |
| `tests/MyApp.Web.Client.Tests` | page tests (bUnit) |

`src/MyApp.Catalog` (brands, products) is the reference module: copy its patterns. `src/MyApp.Documents` shows files on Coworkee.Storage, `src/MyApp.Sales` a client entity with an application service.

Prefer the CLI for new code; it writes all of the above consistently:

```bash
coworkee module add Sales --entity Order --property Number:string:required:50:unique --property Total:money
coworkee entity add Sales Invoice -p Number:required:30 -p Order:ref=Order:required
coworkee migrations add AddSales
```

## Required patterns

- Entities derive from `AuditedEntity` and implement `IMultiTenant` (`Guid TenantId`); add `ISoftDelete` where rows are deleted softly.
- `[Realtime(<Module>Permissions.<Plural>.View)]` on an entity: open tables and detail pages reload when someone else changes it.
- Every use case is a request (`ICommand<Result<T>>` / `IQuery<Result<T>>`), a handler (`IHandler<,>`) and a FluentValidation validator (`AbstractValidator<TRequest>`), sent through `IDispatcher`. Handlers return `Result`/`Error`, never throw for expected failures, and never call `SaveChangesAsync`: the pipeline validates, saves and commits.
- `[RequiresPermission(<Module>Permissions.X)]` on every request that is not meant for everyone. When the permission depends on the input (create vs. edit), check `IPermissionChecker.IsGrantedAsync` in the handler.
- Permission names are constants in `MyApp.Contracts/<Module>/<Module>Permissions.cs`; define each in the module's `IPermissionDefinitionContributor` (group, display name, parent) and register it in the module. Give the demo roles in `DemoSeed.cs` what they need.
- Tables: `IModelContributor` in `Persistence/` with schema `app`, maximum lengths and precisions; register it in the module.
- Endpoints: `app.MapCoworkeeApi("/api/v1/<plural>").RequireAuthorization()`, each one dispatches a request and returns `.ToHttpResult()`. OData sets: `AddODataEntity<T>("<Plural>", <View permission>)`.
- Pages: `@attribute [Authorize(Policy = PermissionPolicy.Prefix + <View permission>)]`, `CoworkeeDataTable` for lists, code in a `.razor.cs` partial next to the `.razor` file. Texts through `L["English text"]`, translations in `MyApp.Contracts/Localization/<culture>.json`.
- Show files with MudBlazor.Extensions' `MudExFileDisplay` (absolute URL); do not build own previews or viewers.

## Pitfalls

- A request without `[RequiresPermission]` (and without a check in its handler) is not checked: any signed-in user may send it, also through MCP and the assistant.
- An entity without `IMultiTenant` is shared by all tenants: no query filter, no tenant on insert.
- `ExecuteUpdate` and `ExecuteDelete` skip the change tracker: no audit entry, no realtime message, no history. Use them only for bulk work and publish yourself (`IRealtimePublisher`).
- Records only some users may see need realtime topics of their own; `[Realtime]` sends to everyone with the permission.

## Migrations

After every model change (entity, property, table mapping, new module):

```bash
coworkee migrations add <Name>
# without the CLI:
dotnet ef migrations add <Name> --project src/MyApp.Infrastructure --startup-project src/MyApp.Infrastructure
```

Never edit an applied migration or the model snapshot by hand. The migrations project applies them at the next `coworkee run`.
`tests/MyApp.Migrations.Tests` fails while the model has changes without a migration.

## OpenAPI and SDKs

`sdk/openapi.json` is a snapshot of the API's OpenAPI document; `tests/MyApp.Api.Tests/OpenApiTests.cs` fails when the API no longer matches it. After changing endpoints or DTOs, refresh it and regenerate the TypeScript client in the same change (see `sdk/README.md`):

```bash
MYAPP_UPDATE_OPENAPI=1 dotnet test --project tests/MyApp.Api.Tests --filter-method "*OpenApi*"
cd sdk/typescript && npm run generate && npm run build
```

## Tests

```bash
dotnet test --solution MyApp.slnx                     # all; needs Docker for the database
dotnet test --project tests/MyApp.Api.Tests           # one project
```

API tests use `ApiFixture` (a real database per run, `ResetAllAsync()` between tests, `As(userId, tenantId)` to act as a user). Prove permission checks (403 without the permission) and tenant separation for new features. Run focused projects; the app host tests start the whole topology one test at a time (collection `AppHost`), keep it that way.

## CLI and MCP

| Command | |
|---|---|
| `coworkee run` | the app host |
| `coworkee module add`, `coworkee entity add`, `coworkee entity import` | modules and entities |
| `coworkee migrations add <Name>` | a migration |
| `coworkee add <option>`, `coworkee remove <option>` | turn template options on or off (`coworkee options` lists them) |
| `coworkee texts export --lang <code>` | library texts to translate |
| `coworkee db reset`, `seed`, `backup`, `restore` | the local database |
| `coworkee update [--dry-run]` | newer Coworkee packages, with the upgrade rules and manual steps |
| `coworkee doctor` | checks the tools |

`coworkee mcp` serves the same commands as MCP tools (`add_module`, `add_entity`, `add_migration`, `add_option`, `update_packages`, `search_docs`, ...): `claude mcp add coworkee -- coworkee mcp`. The running app is an MCP server itself under `/mcp`, authenticated with a personal API key.
