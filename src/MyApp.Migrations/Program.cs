using Coworkee.Core.Modularity;
using Coworkee.Identity.Setup;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure;
using MyApp.Migrations;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddCoworkeeModules<MyAppDatabaseModule>(builder.Configuration);
builder.Services.AddCoworkeeIdentitySeed(DemoSeed.Configure);
builder.Services.AddScoped<Coworkee.Application.Setup.ISetupStep, DemoData>();
using var host = builder.Build();

await using var scope = host.Services.CreateAsyncScope();
try
{
    await scope.ServiceProvider.GetRequiredService<MyAppDbContext>().Database.MigrateAsync();
    await host.Services.SeedCoworkeeIdentityAsync();
    await DemoRoles.EnsureAsync(host.Services);
    return 0;
}
catch (Exception exception)
{
    scope.ServiceProvider.GetRequiredService<ILogger<Program>>().LogCritical(exception, "Database migration or seed failed");
    return 1;
}
