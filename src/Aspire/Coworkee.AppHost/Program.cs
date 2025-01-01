using Aspire.Hosting.Postgres;
using Coworkee.AppHost;
using Coworkee.Application.Configurations;
using Coworkee.Infrastructure;
using Coworkee.Shared.Constants.Application;

var builder = DistributedApplication.CreateBuilder(args);
IResourceBuilder<PgAdminContainerResource> pgAdmin = null;
IResourceBuilder<KeycloakResource> keycloak = null;
IResourceBuilder<OpenWebUIResource>? openWebUi = null;
IResourceBuilder<OllamaResource> ollama = null;
IResourceBuilder<OllamaModelResource> ollamaModel = null;
IResourceBuilder<ContainerResource> grafana = null;
IResourceBuilder<ContainerResource> prometheus = null;

//var db = builder.AddSqlServer("coworkee-sql-server")
//    .WithLifetime(ContainerLifetime.Persistent)
//    .AddDatabase(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection));

var db = builder.AddPostgres("pg")
    .WithPgAdmin(admin =>
    {
        pgAdmin = admin;
    })
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection), "CoworkeeDb");


keycloak = builder.AddKeycloak("keycloak", 8080,
    builder.AddParameter("AdminUserName", ApplicationConstants.Defaults.Users.Administrators[0].UserName),
    builder.AddParameter("AdminUserPassword", ApplicationConstants.Defaults.Users.Administrators[0].Password))
    .WithCommand("Seed Client", "Seed Client", async context =>
    {
        var seeder = new KeycloakSeeder();
        await seeder.CreateClientAsync(true);
        await seeder.CreateUserAsync("hans", "hans", "hans@gmail.com");
        return new ExecuteCommandResult { Success = true };
    });


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

builder.Build().Run();
