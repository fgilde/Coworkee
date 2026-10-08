# MyApp SDKs

Both clients are built from `openapi.json`, the API description the MyApp API serves at `/openapi/v1.json`.
The test `OpenApiTests` fails when the API changes and the file is out of date.

## Update after API changes

```bash
MYAPP_UPDATE_OPENAPI=1 dotnet test tests/MyApp.Api.Tests --filter-class "*OpenApiTests"
cd sdk/typescript && npm install && npm run generate && npm run build
```

## .NET: `MyApp.Client`

`src/MyApp.Client` – typed methods over the DTOs of `MyApp.Contracts` (dashboard, brands, products, document types, documents with upload and download), lists through OData, plus `GetAsync`/`SendAsync`/`QueryAsync` for any other endpoint. Errors raise `CoworkeeApiException` with status, problem code and validation messages.

```csharp
var http = new HttpClient(new BearerTokenHandler(ct => tokens.GetAsync(ct))) { BaseAddress = new Uri("https://myapp.example/") };
var client = new MyAppClient(http);
var brand = await client.SaveBrandAsync(null, new AddEditBrandRequest { Name = "Acme", Tax = 19 });
var cheap = await client.GetProductsAsync("Rate lt 10");
```

## TypeScript: `@myapp/client`

`sdk/typescript` – types generated with `openapi-typescript`, requests through `openapi-fetch`; paths, parameters, bodies and responses are checked by the compiler.

```ts
import { createMyAppClient } from "@myapp/client";

const api = createMyAppClient("https://myapp.example", () => getToken());
const { data: brand } = await api.GET("/api/v1/brands/{id}", { params: { path: { id } } });
```

Authentication: a bearer token of the MyApp API, or the session cookie when the code runs inside the MyApp web app.
