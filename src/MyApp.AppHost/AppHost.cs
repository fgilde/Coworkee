using Coworkee.Aspire.Settings;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddCoworkeeApp("myapp", options =>
{
    options.DisplayName = "MyApp";
    options.LogoUrl = "/coworkee-icon.svg";
    options.UseKeycloak(keycloak => keycloak.Users.Add(new KeycloakUser("info@coworkee.de", "Administrator", "MyApp")));

    // self registration like the standalone app: on, confirmed by email, activated by an administrator (admins change it under Settings)
    options.Configure("auth", Registration).Configure("api", Registration);
}).AddProjects();

builder.Build().Run();

static void Registration(IResourceBuilder<ProjectResource> service) => service
    .WithSetting(s => s.Coworkee.Settings.Defaults["Account.AllowRegistration"], true)
    .WithSetting(s => s.Coworkee.Settings.Defaults["Account.RegistrationRequiresActivation"], true)
    .WithSetting(s => s.Coworkee.Settings.Defaults["Account.RegistrationRequiresEmailConfirmation"], true);
