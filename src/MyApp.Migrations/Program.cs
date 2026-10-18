using Coworkee.Core.Modularity;
using Coworkee.Identity.Setup;
using MyApp.Infrastructure;
using MyApp.Migrations;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddCoworkeeModules<MyAppDatabaseModule>(builder.Configuration);
var deployment = InitialAdmin.From(builder.Configuration) is not null;
builder.AddCoworkeeMigrations<MyAppDbContext>(options =>
{
    // the demo users in every run, unless a deployment passes its first administrator (deploy parameters)
    options.DevelopmentUsers = true;
    options.DevelopmentSeed = DemoSeed.Configure;
    options.DeploymentSeed = DemoSeed.ConfigureDeployment;
    if (!deployment)
    {
        options.AfterSeed = async (services, cancellationToken) =>
        {
            await DemoRoles.EnsureAsync(services, cancellationToken);
            await DemoSales.EnsureAsync(services, cancellationToken);
            await DemoReports.EnsureAsync(services, cancellationToken);
            await DemoWorkflows.EnsureAsync(services, cancellationToken);
            await DemoWebhooks.EnsureAsync(services, cancellationToken);
        };
    }
});
builder.Services.AddScoped<Coworkee.Application.Setup.ISetupStep, DemoData>();
using var host = builder.Build();

return await host.RunCoworkeeMigrationsAsync();
