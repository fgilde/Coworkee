using Coworkee.Core.Modularity;
using Coworkee.Identity.Setup;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure;
using MyApp.Migrations;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddCoworkeeModules<MyAppDatabaseModule>(builder.Configuration);
// the demo users, unless a deployment passes its first administrator (deploy parameters)
var admin = InitialAdmin.From(builder.Configuration);
builder.Services.AddCoworkeeIdentitySeed(admin is null ? DemoSeed.Configure : options => DemoSeed.ConfigureDeployment(options, admin));
builder.Services.AddScoped<Coworkee.Application.Setup.ISetupStep, DemoData>();
using var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();
try
{
    await scope.ServiceProvider.GetRequiredService<MyAppDbContext>().Database.MigrateAsync();
    await host.Services.SeedCoworkeeIdentityAsync();
    if (admin is null)
    {
        // a database set up earlier gets the users and roles added to DemoUsers.cs since, and their passwords again
        await host.Services.SyncDevelopmentUsersAsync();
        await DemoRoles.EnsureAsync(host.Services);
        await DemoSales.EnsureAsync(host.Services);
        await DemoReports.EnsureAsync(host.Services);
        await DemoWorkflows.EnsureAsync(host.Services);
        await DemoWebhooks.EnsureAsync(host.Services);
    }

    // the Keycloak of a deployment gets the clients and the administrator of the app host
    await scope.ServiceProvider.ReconcileKeycloakRealmAsync();
    return 0;
}
catch (Exception exception)
{
    scope.ServiceProvider.GetRequiredService<ILogger<Program>>().LogCritical(exception, "Database migration or seed failed");
    return 1;
}
