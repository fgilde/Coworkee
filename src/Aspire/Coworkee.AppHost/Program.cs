using Aspire.Hosting.Postgres;
using Coworkee.AppHost;
using Coworkee.Application.Configurations;
using Coworkee.Infrastructure;
using Coworkee.Shared.Constants.Application;

// #### CONSTANTS Settings #####################################################

DatabaseToUse databaseToUse = DatabaseToUse.SqlServer;
var administrator = ApplicationConstants.Defaults.Users.Administrators[0];

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


keycloak = builder.AddKeycloak("keycloak", 8080,
    builder.AddParameter("AdminUserName", administrator.UserName),
    builder.AddParameter("AdminUserPassword", administrator.Password))
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
            ApplicationConstants.ApplicationClientSecret
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

ollamaModel = ollama.AddModel(ApplicationConstants.LargeLanguageModel);

grafana = builder.AddContainer("grafana", "grafana/grafana")
    .WithEnvironment("GF_SECURITY_ADMIN_USER", administrator.UserName)
    .WithEnvironment("GF_SECURITY_ADMIN_EMAIL", administrator.Email)
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", administrator.Password)
    .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "false") 
    .WithBindMount("grafana/config", "/etc/grafana", isReadOnly: true)
    .WithBindMount("grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
    .WithHttpEndpoint(targetPort: 3000, name: "http");

prometheus = builder.AddContainer("prometheus", "prom/prometheus")
    .WithBindMount("prometheus", "/etc/prometheus", isReadOnly: true)
    .WithHttpEndpoint(/* This port is fixed as it's referenced from the Grafana config */ port: 9090, targetPort: 9090)
    .WithContainerRuntimeArgs("--network=host");


var api = builder.AddProject<Projects.Server>("coworkee-application")
    .WithEnvironment($"{nameof(ServerConfiguration.PublicSettings)}__{nameof(ServerConfiguration.PublicSettings.Endpoints)}__{nameof(ServerConfiguration.PublicSettings.Endpoints.Grafana)}", grafana?.GetEndpoint("http"))
    .WithEnvironment($"{nameof(ServerConfiguration.PublicSettings)}__{nameof(ServerConfiguration.PublicSettings.Endpoints)}__{nameof(ServerConfiguration.PublicSettings.Endpoints.Ollama)}", ollama?.GetEndpoint("http"))
    .WithEnvironment($"{nameof(ServerConfiguration.PublicSettings)}__{nameof(ServerConfiguration.PublicSettings.Endpoints)}__{nameof(ServerConfiguration.PublicSettings.Endpoints.Prometheus)}", prometheus?.GetEndpoint("http"))
    .WithEnvironment($"{nameof(ServerConfiguration.PublicSettings)}__{nameof(ServerConfiguration.PublicSettings.Endpoints)}__{nameof(ServerConfiguration.PublicSettings.Endpoints.OllamaUI)}", openWebUi?.GetEndpoint("http"))
    .WithEnvironment($"{nameof(ServerConfiguration.PublicSettings)}__{nameof(ServerConfiguration.PublicSettings.Endpoints)}__{nameof(ServerConfiguration.PublicSettings.Endpoints.PGAdmin)}", pgAdmin?.GetEndpoint("http"))
    .WithEnvironment($"{nameof(ServerConfiguration.PublicSettings)}__{nameof(ServerConfiguration.PublicSettings.Endpoints)}__{nameof(ServerConfiguration.PublicSettings.Endpoints.Keycloak)}", keycloak?.GetEndpoint("http"))
    .WithReferenceIf(db)
    .WaitForIf(db)
    .WithReferenceIf(keycloak)
    .WaitForCompletionIf(keycloak)
    .WithReferenceIf(ollama)
    .WithReferenceIf(ollamaModel);

//builder.AddProject<Projects.Client>("coworkee-client")
//    .WaitFor(api)
//    .WithReference(api);


prometheus?.WithReference(api)?.WaitFor(api);

builder
    .Build()
    .EnsureDockerRunningIfLocalDebug()
    .Run();
