using Coworkee.Aspire.Settings;
using MyApp.Migrations;
using Nextended.Aspire.Hosting.AspireUI;

var builder = DistributedApplication.CreateBuilder(args);

var administrator = DemoUsers.All.First(u => u.IsAdmin);

var app = builder.AddCoworkeeApp("myapp", options =>
{
    options.DisplayName = "MyApp";
    options.LogoUrl = "/coworkee-icon.svg";

    // Web Push for the notifications: the VAPID key pair is made at the first start and kept in the user secrets
    options.UsePush();

    // the tools take the sign-in of the app's administrator (LOGIN.md); a deployment takes the administrator of its parameters
    options.Administrator = new(administrator.Email, administrator.Password);
    options.UseGrafana();
    options.UseWebDataStudio();

    // the same users and passwords as the app's seed (LOGIN.md)
    options.UseKeycloak(keycloak => keycloak.Users.AddRange(DemoUsers.All.Select(u => new KeycloakUser(u.Email, u.FirstName, u.LastName ?? "MyApp") { Password = u.Password, Roles = u.IsAdmin ? ["Administrator", .. u.Roles ?? []] : u.Roles ?? [] })));

    // self registration like the standalone app: on, confirmed by email, activated by an administrator (admins change it under Settings)
    options.Configure("auth", Registration).Configure("api", Registration);

    // free machine translation without an account: French, Spanish and Italian are translated at the first start
    string[] languages = ["en", "de", "fr", "es", "it"];
    options.UseLibreTranslate(languages);
    options.Configure("auth", s => OfferLanguages(s, languages)).Configure("api", s => OfferLanguages(s, languages));
}).AddProjects();

// the apps and stacks hosted with AspireUI show up under Services; the administrator signs in locally or through Keycloak (LOGIN.md)
if (app.Deploys(CoworkeeTools.AspireUI))
{
    var aspireUi = builder.AddAspireUI().WithCoworkee(app).WithRetrieveApi();
    if (builder.ExecutionContext.IsRunMode)
    {
        aspireUi.WithSeedFromDirectory(builder.AppHostDirectory);
    }
}

builder.Build().Run();

static void OfferLanguages(IResourceBuilder<ProjectResource> service, string[] languages)
{
    for (var i = 0; i < languages.Length; i++)
    {
        service.WithEnvironment($"Coworkee__Localization__Languages__{i}", languages[i]);
    }
}

static void Registration(IResourceBuilder<ProjectResource> service) => service
    .WithSetting(s => s.Coworkee.Settings.Defaults["Account.AllowRegistration"], true)
    .WithSetting(s => s.Coworkee.Settings.Defaults["Account.RegistrationRequiresActivation"], true)
    .WithSetting(s => s.Coworkee.Settings.Defaults["Account.RegistrationRequiresEmailConfirmation"], true);
