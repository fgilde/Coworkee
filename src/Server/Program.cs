using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coworkee.Infrastructure.Contexts;
using Coworkee.Server.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.Linq;
using Coworkee.Shared.Constants.Application;

namespace Coworkee.Server
{
    public class Program
    {
        private static CancellationTokenSource _cts = new();
        
        private static void Restart()
        {
            _cts.Cancel();
        }

        public static async Task Main(string[] args)
        {
            ApplicationConstants.IsNswagGeneration = args.Any(arg => arg.Contains("--applicationName", StringComparison.OrdinalIgnoreCase));
            do
            {
                await StartServer(args);
                Console.WriteLine("Restarting App");
            } while (_cts.IsCancellationRequested);
        }

        public static async Task StartServer(string[] args)
        {
            _cts = new CancellationTokenSource();

            var builder = WebApplication.CreateBuilder(args);
            builder.AddServiceDefaults();
            builder.Configuration.AddConfigurations();

            builder.WebHost.UseStaticWebAssets();

            var startup = new Startup(builder.Configuration);

            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await TryMigrateDbAsync(services, scope);
                services.GetService<IHostApplicationLifetime>()?.ApplicationStopping.Register(Restart);
            }

            startup.Configure(app, app.Environment, app.Services.GetService<IStringLocalizer<Startup>>(), app.Services.GetService<IDashboardAuthorizationFilter>());
            app.MapDefaultEndpoints();
            try
            {
                await app.RunAsync(_cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static async Task TryMigrateDbAsync(IServiceProvider services, IServiceScope scope)
        {
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                if (context.Database.IsSqlServer() || context.Database.IsNpgsql())
                {
                    var needsMigration = (await context.Database.GetPendingMigrationsAsync()).Any() || context.Database.HasPendingModelChanges();
                    if (needsMigration)
                        await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                scope.ServiceProvider.GetRequiredService<ILogger<Program>>()
                    .LogError(ex, "Error occurred while migrating/seeding database.");
               // throw;
            }
        }
    }
}
