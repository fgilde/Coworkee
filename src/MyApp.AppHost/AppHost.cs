var builder = DistributedApplication.CreateBuilder(args);

var app = builder.AddCoworkeeApp("myapp", options =>
{
    options.DisplayName = "MyApp";
    options.UseKeycloak(keycloak => keycloak.Users.Add(new KeycloakUser("info@coworkee.de", "Administrator", "MyApp")));
});

app.AddMigrations<Projects.MyApp_Migrations>();
app.AddAuthServer<Projects.MyApp_Auth>();
app.AddApi<Projects.MyApp_Api>();
app.AddWeb<Projects.MyApp_Web>();

builder.Build().Run();
