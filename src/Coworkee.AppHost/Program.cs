var builder = DistributedApplication.CreateBuilder(args);
var p = builder.AddParameter("p", "SUPERP");

var db = builder.AddSqlServer("coworkee-sql-server")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("Database");

//var db = builder.AddPostgres("pg")
//    .WithPgAdmin()
//    .AddDatabase("postgresdb", "CoworkeeDb");

var api = builder.AddProject<Projects.Server>("coworkee-server")
    .WithReference(db)
    .WaitFor(db);


builder.AddProject<Projects.Client>("coworkee-client")
    .WaitFor(api)
    .WithReference(api);


builder.Build().Run();
