using Coworkee.Core.Modularity;
using Coworkee.Identity.Setup;
using MyApp.Infrastructure;
using MyApp.Migrations;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddCoworkeeModules<MyAppDatabaseModule>(builder.Configuration);
builder.AddCoworkeeMigrations<MyAppDbContext>(options =>
{
    // the demo users in every run, unless a deployment passes its first administrator (deploy parameters)
    options.DevelopmentUsers = true;
    options.DevelopmentSeed = DemoSeed.Configure;
    options.DeploymentSeed = DemoSeed.ConfigureDeployment;
});
builder.Services.AddScoped<Coworkee.Application.Setup.ISetupStep, DemoData>();
using var host = builder.Build();

var exitCode = await host.RunCoworkeeMigrationsAsync();
if (exitCode != 0 || InitialAdmin.From(builder.Configuration) is not null)
{
    return exitCode;
}

try
{
    await DemoRoles.EnsureAsync(host.Services);
    await DemoSales.EnsureAsync(host.Services);
    await DemoReports.EnsureAsync(host.Services);
    await DemoWorkflows.EnsureAsync(host.Services);
    await DemoWebhooks.EnsureAsync(host.Services);
    return 0;
}
catch (Exception exception)
{
    host.Services.GetRequiredService<ILogger<Program>>().LogCritical(exception, "Demo data failed");
    return 1;
}
