var builder = DistributedApplication.CreateBuilder(args);

//var db = builder.AddSqlServer("coworkee-sql-server")
//    .WithLifetime(ContainerLifetime.Persistent)
//    .AddDatabase("DefaultConnection");

var db = builder.AddPostgres("pg")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("postgresdb", "CoworkeeDb");


//var keycloak = builder.AddKeycloak("keycloak", 8080,
//    builder.AddParameter("AdminUserName", "admin"),
//    builder.AddParameter("AdminUserPassword", "cargonerds123"));

//var grafana = builder.AddContainer("grafana", "grafana/grafana")
//    .WithBindMount("../grafana/config", "/etc/grafana", isReadOnly: true)
//    .WithBindMount("../grafana/dashboards", "/var/lib/grafana/dashboards", isReadOnly: true)
//    .WithHttpEndpoint(targetPort: 3000, name: "http");

//builder.AddContainer("prometheus", "prom/prometheus")
//    .WithBindMount("../prometheus", "/etc/prometheus", isReadOnly: true)
//    .WithHttpEndpoint(/* This port is fixed as it's referenced from the Grafana config */ port: 9090, targetPort: 9090);



var ollama = builder.AddOllama("ollama")
    .WithContainerRuntimeArgs()
    .WithDataVolume()
    .WithOtlpExporter()
    .WithOpenWebUI(webui =>
    {
        webui.WithOtlpExporter()
            .WithExternalHttpEndpoints()
            .PublishAsContainer();
    })
    .WithExternalHttpEndpoints()
    .PublishAsContainer()
    .AddModel("llama3.2");

var api = builder.AddProject<Projects.Server>("coworkee-application")
    .WithReference(db)
    .WaitFor(db);


//builder.AddProject<Projects.Client>("coworkee-client")
//    .WaitFor(api)
//    .WithReference(api);


builder.Build().Run();
