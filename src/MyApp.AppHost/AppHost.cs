using Microsoft.Extensions.Hosting;
var builder = DistributedApplication.CreateBuilder(args);

var infrastructure = builder.AddCoworkeeInfrastructure("myapp");

var migrations = builder.AddProject<Projects.MyApp_Migrations>("myapp-migrations")
    .WithReference(infrastructure.Database)
    .WaitFor(infrastructure.Server);

// uploaded documents
var blobRoot = Path.GetFullPath(Path.Combine(builder.AppHostDirectory, "..", "..", ".data", "blobs"));

var auth = builder.AddProject<Projects.MyApp_Auth>("myapp-auth")
    .WithReference(infrastructure.Database)
    .WaitForCompletion(migrations)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

if (builder.Environment.IsDevelopment())
{
    // per machine certificates are fine locally; production configures Coworkee__Auth__SigningCertificate and __EncryptionCertificate
    auth.WithEnvironment("Coworkee__Auth__DevelopmentCertificates", "true");
}

var api = builder.AddProject<Projects.MyApp_Api>("myapp-api")
    .WithReference(infrastructure.Database)
    .WaitForCompletion(migrations)
    .WithEnvironment("Auth__Authority", auth.GetEndpoint("https"))
    .WithEnvironment("Coworkee__Account__PublicAuthUrl", auth.GetEndpoint("https"))
    .WithEnvironment("Coworkee__Settings__Defaults__Mail.Smtp.Host", infrastructure.Mail.GetEndpoint("smtp").Property(EndpointProperty.Host))
    .WithEnvironment("Coworkee__Settings__Defaults__Mail.Smtp.Port", infrastructure.Mail.GetEndpoint("smtp").Property(EndpointProperty.Port))
    .WaitFor(infrastructure.Mail)
    .WithReference(infrastructure.Redis)
    .WaitFor(infrastructure.Redis)
    .WithEnvironment("Coworkee__Storage__FileSystem__Root", blobRoot)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

if (builder.Configuration["MyApp:SetupToken"] is { Length: > 0 } setupToken)
{
    api.WithEnvironment("Coworkee__SetupToken", setupToken);
}

var web = builder.AddProject<Projects.MyApp_Web>("myapp-web")
    .WithReference(api)
    .WaitFor(auth)
    .WithEnvironment("Coworkee__Bff__Authority", auth.GetEndpoint("https"))
    .WithEnvironment("Coworkee__Bff__ClientId", "myapp-web")
    .WithEnvironment("Coworkee__Bff__ApiAddress", "https+http://myapp-api")
    .WithEnvironment("Coworkee__Bff__Scopes__0", "myapp_api")
    .WithEnvironment("Coworkee__Bff__ForwardedPrefixes__0", "/admin/jobs")
    .WithEnvironment("Coworkee__Bff__ForwardedPrefixes__1", "/hubs")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

auth.WithEnvironment("Coworkee__Auth__ApiScopes__myapp_api", "myapp_api")
    .WithEnvironment("Coworkee__Jobs__RunServer", "false")
    .WithEnvironment("Coworkee__Account__PublicAuthUrl", auth.GetEndpoint("https"))
    .WithEnvironment("Coworkee__Auth__Clients__0__ClientId", "myapp-web")
    .WithEnvironment("Coworkee__Auth__Clients__0__DisplayName", "MyApp")
    .WithEnvironment("Coworkee__Auth__Clients__0__Scopes__0", "myapp_api")
    .WithEnvironment("Coworkee__Auth__Clients__0__RedirectUris__0", ReferenceExpression.Create($"{web.GetEndpoint("https")}/signin-oidc"))
    .WithEnvironment("Coworkee__Auth__Clients__0__PostLogoutRedirectUris__0", ReferenceExpression.Create($"{web.GetEndpoint("https")}/signout-callback-oidc"));

builder.Build().Run();
