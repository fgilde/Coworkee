using Aspire.Hosting.Azure;
using Coworkee.AppHost;
using Coworkee.AppHost.OpenTelemetryCollector;
using Coworkee.Application.Configurations;
using Coworkee.Shared.Constants.Application;
using Microsoft.Extensions.Hosting;
using Nextended.Aspire;
using System.Runtime.CompilerServices;
using Coworkee.AppHost.Helper;
using Coworkee.Infrastructure;
using C = Coworkee.Shared.Constants.Application.ApplicationConstants;

// TODO: LanguageModel Path-chat
// #### CONSTANTS Settings #####################################################

DatabaseToUse databaseToUse = DatabaseToUse.Postgres;
var administrator = C.Defaults.Users.Administrators[0];

bool ollamaEnabled = true;
bool keycloakEnabled = true;
bool grafanaEnabled = true;
bool stirlingEnabled = true;
bool storageEnabled = true;

// ####### Start the Aspire application ########################################

var builder = DistributedApplication.CreateBuilder(args);

var userNameParam = builder.AddParameter("AdminUserName", administrator.UserName);
var userPasswordParam = builder.AddParameter("AdminUserPassword", administrator.Password);
//var dbUsername = builder.AddParameter("username","dbUser", secret: true);
//var dbPassword = builder.AddParameter("password","dbPassword", secret: true);

//IResourceBuilder<PgAdminContainerResource> pgAdmin = null;
IResourceBuilder<KeycloakResource>? keycloak = null;
IResourceBuilder<OpenWebUIResource>? openWebUi = null;
IResourceBuilder<OllamaResource> ollama = null;
IResourceBuilder<OllamaModelResource> ollamaModel = null;
IResourceBuilder<ContainerResource> grafana = null;
IResourceBuilder<ContainerResource> prometheus = null;
IResourceBuilder<ContainerResource> stirling = null;
IResourceBuilder<AzureStorageResource> storage = null;
IResourceBuilder<AzureBlobStorageResource> blobs = null;

var signalr = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSignalR(C.SignalR.Resource)
    : null;



var db = builder.WithDatabase(databaseToUse);


if (keycloakEnabled)
{
    keycloak = builder.WithKeycloak(C.ServiceNames.Keycloak, userNameParam, userPasswordParam);
}


if (storageEnabled)
{
    storage = builder.WithStorage(out blobs);
}


if (ollamaEnabled)
{
    var res = builder.WithOllama();
    ollama = res.OllamaResource;
    openWebUi = res.OpenWebUIResource;
    ollamaModel = res.OllamaModelResource;
}


if (grafanaEnabled)
{
    var res = builder.WithGrafana(userNameParam, userPasswordParam, administrator);
    prometheus = res[0];
    grafana = res[1];
}

if (stirlingEnabled)
{
    stirling = builder.AddContainer(C.ServiceNames.Stirling, "stirlingtools/stirling-pdf")
        .WithHttpEndpoint(targetPort: 8080, name: "http")
        .WithHttpHealthCheck("/", 200)
        .WithExternalHttpEndpoints();
}

var api = builder.AddProject<Projects.Server>(ApplicationConstants.AspireServerAppName)
    .WithEndpointsAsEnvironmentIf<ProjectResource, ServerConfiguration>(s => s.PublicSettings.Endpoints, [grafana, ollama, prometheus, openWebUi, db.AdminUIResource, keycloak, stirling])
    .WithReferenceIf(db.DatabaseResource)
    .WaitForIf(db.DatabaseResource)
    .WaitForCompletionIf(stirling)
    .WithReferenceIf(keycloak)
    .WaitForCompletionIf(keycloak)
    .WithReferenceIf(ollama)
    .WithReferenceIf(ollamaModel)
    .WithReferenceIf(blobs)
    .WithReferenceIf(signalr)
    .WithExternalHttpEndpoints();

if (!ApplicationConstants.HostClientInServer)
{
    api.AddWebAssemblyClient<Projects.Client>(ApplicationConstants.AspireClientAppName).WithReference(api);
    if (!builder.ExecutionContext.IsRunMode)
    {
        // TODO: on azd up we can use the following but currently we need the build arg BACKEND_ORIGIN and we cant use api.GetEndpoint("http") here
        //builder.AddDockerfile(ApplicationConstants.AspireClientAppName, "../../../", "./src/Client/Dockerfile")
        //    .WaitFor(api)
        //    .WithReference(api)
        //    .WithHttpEndpoint(env: "PORT", name: "http", targetPort: 80)        
        //    .WithBuildArg("BACKEND_ORIGIN", "https://localhost:5001")
        //    .WithExternalHttpEndpoints()
        //    .WithHttpHealthCheck("/", 200)
        //    .WithOtlpExporter()
        //    .PublishAsContainer();
    }
}


prometheus?.WithReference(api)?.WaitFor(api);

builder
    .Build()
    .EnsureDockerRunningIfLocalDebug()
    .Run();
