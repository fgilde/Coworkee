var builder = DistributedApplication.CreateBuilder(args);

builder.AddCoworkeeApp("myapp", options =>
{
    options.DisplayName = "MyApp";
    options.UseKeycloak(keycloak => keycloak.Users.Add(new KeycloakUser("info@coworkee.de", "Administrator", "MyApp")));
}).AddProjects();

builder.Build().Run();
