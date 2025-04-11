using Coworkee.AppHost;
using Coworkee.Application.Configurations;
using Coworkee.Shared.Constants.Application;
using Nextended.Aspire;
using Coworkee.AppHost.ApplicationServiceHelper;
using Coworkee.AppHost.Types;

// #### Settings #####################################################

var settings = new CoworkeeAppHostSettings()
{
    DatabaseToUse = DatabaseToUse.Postgres,
    AddOllama = true,
    AddKeycloak = true,
    AddGrafana = true,
    AddStirling = true,
    AddAzureStorage = true,
};


// ####### Start the Aspire application ##############################

var builder = DistributedApplication.CreateBuilder(args);
var services = builder.AddDependencyServices(settings);
var prometheus = services.OfType<IResourceBuilder<ContainerResource>>().FirstOrDefault(s => s.Resource.Name == ApplicationConstants.ServiceNames.Prometheus);
var ollamaModel = services.OfType<IResourceBuilder<OllamaModelResource>>().FirstOrDefault();

var api = builder.AddProject<Projects.Server>(ApplicationConstants.AspireServerAppName)
    .WithEndpointsAsEnvironmentIf<ProjectResource, ServerConfiguration>(s => s.PublicSettings.Endpoints, services.OfType<IResourceBuilder<IResourceWithEndpoints>>().ToArray())
    .WithEnvironment($"{nameof(ServerConfiguration.PublicSettings)}__{nameof(ServerConfiguration.PublicSettings.Endpoints)}__{nameof(ApplicationConstants.Routes.Dashboard)}", ApplicationConstants.Routes.Dashboard)
    .WaitForIf(services.Except([prometheus, ollamaModel]).ToArray())
    .WithReferencesIf(services.OfType<IResourceBuilder<IResourceWithConnectionString>>().ToArray())
    .WithReferencesIf(services.OfType<IResourceBuilder<IResourceWithServiceDiscovery>>().ToArray())
    .WithExternalHttpEndpoints();

prometheus?.WithReference(api).WaitFor(api);

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


//prometheus?.WithReference(api)?.WaitFor(api);

builder
    .Build()
    .EnsureDockerRunningIfLocalDebug()
    .Run();