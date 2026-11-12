using Coworkee.Core.Modularity;
using Coworkee.Identity.Setup;
using MyApp.Infrastructure;
using MyApp.Migrations;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddCoworkeeModules<MyAppDatabaseModule>(builder.Configuration);
// the demo users in every run, unless a deployment passes its first administrator (deploy parameters); the Demo* seed contributors of this
// project ([RegisterAs]) follow: the sample catalog when the system is set up, the rest of the demo data in development runs only
builder.AddCoworkeeMigrations<MyAppDbContext>(options =>
{
    options.DevelopmentUsers = true;
    options.DevelopmentSeed = DemoSeed.Configure;
    options.DeploymentSeed = DemoSeed.ConfigureDeployment;
});
using var host = builder.Build();

return await host.RunCoworkeeMigrationsAsync();
