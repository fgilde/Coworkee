using Coworkee.Core.Modularity;
using Coworkee.Identity;
using Coworkee.Application.Setup;
using Coworkee.Identity.Setup;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application;

namespace MyApp.Infrastructure;

[DependsOn(typeof(MyAppApplicationModule), typeof(CoworkeeIdentityModule))]
public sealed class MyAppInfrastructureModule : CoworkeeModule
{
    public const string ConnectionStringName = "myapp";

    public override void ConfigureServices(ModuleServiceContext context)
    {
        context.Services.AddCoworkeeDbContext<MyAppDbContext>((provider, options) => options.UseNpgsql(
            provider.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStringName),
            npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "cw")));
        context.Services.AddHealthChecks().AddDbContextCheck<MyAppDbContext>("database");
        context.Services.Configure<SetupGateOptions>(options => options.AllowedPrefixes.Add("/api/v1/system"));
        context.Services.PostConfigure<Coworkee.Contracts.Configuration.BackgroundJobOptions>(options => options.ConnectionStringName = ConnectionStringName);
    }
}
