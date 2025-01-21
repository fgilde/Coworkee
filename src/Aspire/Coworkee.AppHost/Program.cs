using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Postgres;
using Coworkee.AppHost;
using Coworkee.AppHost.OpenTelemetryCollector;
using Coworkee.Application.Configurations;
using Coworkee.Infrastructure;
using Coworkee.Shared.Constants.Application;

// #### CONSTANTS Settings #####################################################

DatabaseToUse databaseToUse = DatabaseToUse.SqlServer;
var administrator = ApplicationConstants.Defaults.Users.Administrators[0];

bool ollamaEnabled = true;
bool keycloakEnabled = true;
bool grafanaEnabled = true;


// ####### Start the Aspire application ########################################

var builder = DistributedApplication.CreateBuilder(args);
IResourceBuilder<PgAdminContainerResource> pgAdmin = null;
IResourceBuilder<KeycloakResource> keycloak = null;
IResourceBuilder<OpenWebUIResource>? openWebUi = null;
IResourceBuilder<OllamaResource> ollama = null;
IResourceBuilder<OllamaModelResource> ollamaModel = null;
IResourceBuilder<ContainerResource> grafana = null;
IResourceBuilder<ContainerResource> prometheus = null;

//var username = builder.AddParameter("username","dbUser", secret: true);
//var password = builder.AddParameter("password","dbPassword", secret: true);

IResourceBuilder<IResourceWithConnectionString> db = databaseToUse switch
{
    DatabaseToUse.Postgres => builder.AddPostgres("pg" //, username, password
    )
        .WithPgAdmin(admin => {
            admin.WithEnvironment("PGADMIN_CONFIG_SERVER_MODE", "True");
            admin.WithEnvironment("PGADMIN_DEFAULT_EMAIL", administrator.Email);
            admin.WithEnvironment("PGADMIN_DEFAULT_PASSWORD", administrator.Password);
            pgAdmin = admin; 
        })        
        .WithLifetime(ContainerLifetime.Persistent)
        .AddDatabase(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection), "CoworkeeDb"),
    DatabaseToUse.SqlServer => builder.AddSqlServer("coworkee-sql-server")
        .WithLifetime(ContainerLifetime.Persistent)
        .AddDatabase(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection))
};

if (keycloakEnabled)
{
    // TODO: azd up (Certificate)
    keycloak = builder.AddKeycloak("keycloak", 8080,
        builder.AddParameter("AdminUserName", administrator.UserName),
        builder.AddParameter("AdminUserPassword", administrator.Password))
        .WithArgs("--features=preview")
        .WithDataVolume()
        //.RunWithHttpsDevCertificate()
        //.RunKeycloakWithHttpsDevCertificate(port: 8081) // Own extension method only works locally and breaks currently all other resources
        .WithCommand("Seed Client", "Seed Client", async context =>
        {
            bool result;
            string message = "";
            var seeder = new KeycloakSeeder(
                ApplicationConstants.KeycloakRealm,
                administrator.UserName,
                administrator.Password,
                keycloak.GetEndpoint("http").Url,
                ApplicationConstants.ApplicationClientName,
                ApplicationConstants.Defaults.ApplicationClientSecret
            );
            try
            {
                await seeder.CreateClientAsync(true);
                await seeder.CreateUserAsync(administrator.UserName, administrator.Password, administrator.Email);
                await seeder.CreateUserAsync("hans", "hans", "hans@gmail.com");
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
            }
            return new ExecuteCommandResult { Success = result, ErrorMessage = message };
        }).WithHttpHealthCheck("/", 200);
}

if (ollamaEnabled)
{
    ollama = builder.AddOllama("ollama")
        .WithContainerRuntimeArgs()
        .WithDataVolume()
        .WithOtlpExporter()
        .WithOpenWebUI(webui =>
        {
            openWebUi = webui;
            webui.WithOtlpExporter()
                .WithExternalHttpEndpoints()
                .PublishAsContainer();
        })
        .WithExternalHttpEndpoints()
        .PublishAsContainer();

    ollamaModel = ollama.AddModel(ApplicationConstants.LargeLanguageModel); // TODO: azd up (UI is available and ollama as well but the model is not loaded or not available. In dahboard is also no ModelResource visible, but local it is)
}

if (grafanaEnabled)
{

    prometheus = builder.AddContainer("prometheus", "prom/prometheus")
        .WithBindMount("prometheus", "/etc/prometheus", isReadOnly: true) // TODO: azd up (Folder and file paths not working on deployed azure container cluster)
        .WithArgs("--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml")
        .WithHttpEndpoint(targetPort: 9090, name: "http");

    grafana = builder.AddContainer("grafana", "grafana/grafana")
        .WithEnvironment("GF_SECURITY_ADMIN_USER", administrator.UserName)
        .WithEnvironment("GF_SECURITY_ADMIN_EMAIL", administrator.Email)
        .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", administrator.Password)
        .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "false")
        .WithBindMount("grafana/config", "/etc/grafana", isReadOnly: true) // TODO: azd up (Folder and file paths not working on deployed azure container cluster)
        .WithBindMount("grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true) // TODO: azd up (Folder and file paths not working on deployed azure container cluster)
        .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"))
        .WithHttpEndpoint(targetPort: 3000, name: "http");

    builder.AddOpenTelemetryCollector("otelcollector", "otelcollector/config.yaml")
        .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/otlp");

}

var api = builder.AddProject<Projects.Server>(ApplicationConstants.AspireServerAppName)
    .WithEndpointAsEnvironmentIf<ProjectResource, ServerConfiguration>(s => s.PublicSettings.Endpoints.Grafana, grafana)
    .WithEndpointAsEnvironmentIf<ProjectResource, ServerConfiguration>(s => s.PublicSettings.Endpoints.Ollama, ollama)
    .WithEndpointAsEnvironmentIf<ProjectResource, ServerConfiguration>(s => s.PublicSettings.Endpoints.Prometheus, prometheus)
    .WithEndpointAsEnvironmentIf<ProjectResource, ServerConfiguration>(s => s.PublicSettings.Endpoints.OllamaUI, openWebUi)
    .WithEndpointAsEnvironmentIf<ProjectResource, ServerConfiguration>(s => s.PublicSettings.Endpoints.PGAdmin, pgAdmin)
    .WithEndpointAsEnvironmentIf<ProjectResource, ServerConfiguration>(s => s.PublicSettings.Endpoints.Keycloak, keycloak)        
    .WithReferenceIf(db)
    .WaitForIf(db)
    .WithReferenceIf(keycloak)
    .WaitForCompletionIf(keycloak)
    .WithReferenceIf(ollama)
    .WithReferenceIf(ollamaModel);

if (!ApplicationConstants.HostClientInServer)
{        
    api.AddWebAssemblyClient<Projects.Client>(ApplicationConstants.AspireClientAppName).WithReference(api);
    if(!builder.ExecutionContext.IsRunMode)
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
