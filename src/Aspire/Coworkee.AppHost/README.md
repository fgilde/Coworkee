
# Coworkee.AppHost
---


### Prerequisites
- **Docker Desktop**: Required for the Aspire setup. [Download Docker Desktop](https://www.docker.com/products/docker-desktop)


---

<details>
  <summary>Running Locally</summary>

  ## Running Locally
This section provides instructions for running the project `Coworkee.AppHost` locally. You can start the project using the command line, Visual Studio, or JetBrains Rider. Additionally, this guide explains how to configure the `Program.cs` file for local development and what the available options mean.

## 1. Running from the Command Line

To start the project from the command line:

1. Navigate to the `Coworkee.AppHost` directory:
   ```bash
   cd path/to/Coworkee.AppHost
   ```

2. Run the project using the .NET CLI:
   ```bash
   dotnet run
   ```

---

## 2. Running in Visual Studio

1. Open the solution in Visual Studio.
2. Set `Coworkee.AppHost` as the **Startup Project**:
   - Right-click on `Coworkee.AppHost` in the Solution Explorer.
   - Select **Set as Startup Project**.
3. Start the project:
   - Press `F5` to run in Debug mode.
   - Press `Ctrl + F5` to run without Debugging.

---

## 3. Running in JetBrains Rider

1. Open the solution in JetBrains Rider.
2. Set `Coworkee.AppHost:http` as the **Runtime Configuration**:   
3. Start the project:
   - Click the **Run** button next to the `Coworkee.AppHost` configuration.
   - Alternatively, press `Ctrl + F5` to run or `Alt + F5` to Debug.

---
</details>

<details>
  <summary>Deploy</summary>

## Deploy

###### Links [Deployment in Pipelines](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/configure-devops-pipeline?tabs=GitHub)


#### Optional generate manifest for Azure Container Instances (ACI) or Azure Kubernetes Service (AKS)

Navigate to folder of the Aspire Apphost project.

`cd src/Aspire/Coworkee.AppHost`

`dotnet run --publisher manifest --output-path manifest.json`

#### AZD Deploy Steps

Ensure azd is installed

`winget install microsoft.azd`

Navigate to Solution directory (where Solution File is located)

Login to azd

`azd auth login`

*Optional*
Enable preview features

`azd config set alpha.azd.operations on`

Deploy

`azd up`

Delete

`azd down`

</details>