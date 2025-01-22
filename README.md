# Coworkee
---
Single Page App (Blazor) and an ASP.NET Core Server following the principles of Clean Architecture.


[A running demo is available here](https://coworkee.azurewebsites.net/)

<details>
  <summary>Technologies</summary>

  ## Technologies

* ASP.NET Core 9  
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)  
* [Signal R](https://docs.microsoft.com/en-US/aspnet/signalr/overview/getting-started/introduction-to-signalr)  
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor)  
* [Mud Blazor](https://mudblazor.com/getting-started/installation#manual-install)  
* [Mud Blazor Extensions](https://www.mudex.org)  
* [MediatR](https://github.com/jbogard/MediatR)  
* [FluentValidation](https://fluentvalidation.net/)  
* [NUnit](https://nunit.org/), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq) & [Respawn](https://github.com/jbogard/Respawn)  
* [Docker](https://www.docker.com/)

</details>

<details>
  <summary>Getting Started</summary>

## Getting Started
1. Install the latest [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2. Navigate to `src/Server` and run `dotnet run` to launch the back end and the webassembly client (ASP.NET Core Web API), or open the Solution in Visual Studio and launch the Server project.
3. Recommended is to use the `Aspire/Coworkee.AppHost` as start up project.
[More info in src/Aspire/Coworkee.AppHost/README.md](src/Aspire/Coworkee.AppHost/README.md)

</details>

<details>
 <summary>Structure Overview</summary>

## Overview 
### Domain
This contains all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.

### Application
This layer contains all application logic. It depends on the domain layer, but has no dependencies on any other layer or project.  
This layer defines interfaces that are implemented by outside layers. For example, if the application needs to access a notification service, a new interface would be added to the Application layer and an implementation would be created within Infrastructure.

### Infrastructure
This layer contains classes for accessing external resources such as file systems, web services, SMTP, and so on.  
These classes should be based on interfaces defined within the Application layer.

### Shared
This layer contains shared resources across the solution. These are for example the automatically translated resources, the email templates with generated codes for it and the shared properties, and the shared settings.

### SDK
This layer contains the SDK for the Coworkee API. Here are the fully generated API for the Coworkee API located. Also the result of this SDK is used in the `Client` project. And provided as downloadable nuget package to allow your users an easy to use SDK/Api approach

### Server
This is the single page application based on Blazor and the Server/API as ASP.NET Core.  
This layer depends on both the Application and Infrastructure layers; the dependency on Infrastructure is only to support dependency injection.  
Therefore, only `Startup.cs` should reference Infrastructure.

### Client 
This is the Blazor WebAssembly client. Depending on the `HostClientInServer` setting in `Shared.props`, the client can be hosted together with the server or run separately.

 </details>


<details>
  <summary>Shared.props configuration</summary>

  ## Shared.props Configuration
The file `src/Shared.props` contains a property that controls whether the Blazor WebAssembly client is hosted together with the server or not:

```xml
<PropertyGroup>
  <HostClientInServer>true</HostClientInServer>
</PropertyGroup>
```

- If `HostClientInServer` is **true**, the server hosts both API and client together.
- If `HostClientInServer` is **false**, the client runs separately.

</details>

<details>
  <summary>Docker</summary>

### Building Docker Images Individually

From the **root** of the solution, you can build the images separately with:

```bash
docker build -t coworkee-server -f src/Server/Dockerfile .
docker build -t coworkee-client -f src/Client/Dockerfile .
```

### Using Docker Compose

Depending on your `HostClientInServer` setting in `src/Shared.props`:

- **If `HostClientInServer` = true**:  
  You only need the default `docker-compose.yml`. Run:  
  ```bash
  docker compose -f docker-compose.yml up --build
  ```

- **If `HostClientInServer` = false**:  
  You need both `docker-compose.yml` **and** `docker-compose.client.yml`. Run:  
  ```bash
  docker compose -f docker-compose.yml -f docker-compose.client.yml up --build
  ```

### Additional Docker Configuration Notes

In order to get Docker working with HTTPS, you will need to add a temporary SSL cert and mount a volume to hold that cert.  
You can find [Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1) that describe the steps required for Windows, macOS, and Linux.

**Windows** example:  
```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Your_password123
dotnet dev-certs https --trust
```

(When using PowerShell, replace `%USERPROFILE%` with `$env:USERPROFILE`.)

**macOS** example:  
```bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123
dotnet dev-certs https --trust
```

**Linux** example:  
```bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p Your_password123
```

Then you can run or debug as usual, for example:  
```bash
docker compose up --build
```
and open http://localhost:5000 in your browser (or the mapped port you configured).

</details>

<details>
  <summary>Database Configuration</summary>

  ## Database Configuration

The template is configured to use an **SQL Local DB** by default when starting the `src/Server/Server.csproj`.
When starting with Docker or Aspire the database is configured to use **SQL Server** by default.
However you easily can switch to a different database provider PostgressSQL is fully supported.

If you're using the `Aspire/Coworkee.AppHost` you can configure the database provider in the [Program.cs](src/Aspire/Coworkee.AppHost/Program.cs) file.
Just change this line and that's it:
```c#
DatabaseToUse databaseToUse = DatabaseToUse.Postgres; 
```

When you run the application, the database will be automatically created (if necessary) and the latest migrations will be applied.

</details>

<details>
  <summary>Database Migrations</summary>

To use `dotnet-ef` for your migrations, please add the following flags to your command (values assume you are executing from the repository root):

* `--project src/Infrastructure` (optional if you run it within that folder)
* `--startup-project src/Server`
* `--output-dir Migrations`

For example, to add a new migration from the root folder:

```bash
dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\Server --output-dir Migrations
```

</details>


<details>
  <summary>Aspire</summary>

## Aspire
This is the recommended way to start the project locally. 
For more information, please see:  
[More info in src/Aspire/Coworkee.AppHost/README.md](src/Aspire/Coworkee.AppHost/README.md)

</details>

