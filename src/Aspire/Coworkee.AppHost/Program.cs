using Coworkee.Application.Configurations;
using Microsoft.Extensions.Azure;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

//var db = builder.AddSqlServer("coworkee-sql-server")
//    .WithLifetime(ContainerLifetime.Persistent)
//    .AddDatabase(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection));

var db = builder.AddPostgres("pg")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection), "CoworkeeDb");


var keycloak = builder.AddKeycloak("keycloak", 8080,
    builder.AddParameter("AdminUserName", "admin"),
    builder.AddParameter("AdminUserPassword", "admin"))
    .WithCommand("Seed Client", "Seed Client", context =>
    {
        return Task.FromResult(new ExecuteCommandResult() { Success = true });
    });


IResourceBuilder<OpenWebUIResource>? openWebUi = null;
var ollama = builder.AddOllama("ollama")
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

var ollamaModel = ollama.AddModel("llama3.3");

var grafana = builder.AddContainer("grafana", "grafana/grafana")
    .WithBindMount("grafana/config", "/etc/grafana", isReadOnly: true)
    .WithBindMount("grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
    .WithHttpEndpoint(targetPort: 3000, name: "http");

var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
    .WithBindMount("prometheus", "/etc/prometheus", isReadOnly: true)
    .WithHttpEndpoint(/* This port is fixed as it's referenced from the Grafana config */ port: 9090, targetPort: 9090)
    .WithContainerRuntimeArgs("--network=host");

var api = builder.AddProject<Projects.Server>("coworkee-application")
    .WithEnvironment($"{nameof(ServerConfiguration.Endpoints)}__{nameof(ServerConfiguration.Endpoints.Grafana)}", grafana.GetEndpoint("http"))
    .WithEnvironment($"{nameof(ServerConfiguration.Endpoints)}__{nameof(ServerConfiguration.Endpoints.Ollama)}", ollama.GetEndpoint("http"))
    .WithEnvironment($"{nameof(ServerConfiguration.Endpoints)}__{nameof(ServerConfiguration.Endpoints.Prometheus)}", prometheus.GetEndpoint("http"))
    .WithEnvironment($"{nameof(ServerConfiguration.Endpoints)}__{nameof(ServerConfiguration.Endpoints.OllamaUI)}", openWebUi?.GetEndpoint("http"))
    .WithReference(db)
    .WaitFor(db)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithReference(ollama)
    .WithReference(ollamaModel);

//builder.AddProject<Projects.Client>("coworkee-client")
//    .WaitFor(api)
//    .WithReference(api);


prometheus
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
