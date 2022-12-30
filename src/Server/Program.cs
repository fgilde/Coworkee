using System;
using System.Threading.Tasks;
using Coworkee.Infrastructure.Contexts;
using Coworkee.Server.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;

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
            await StartServer(args);
            while (_cts.IsCancellationRequested)
            {
                Console.WriteLine("Restarting App");
                await StartServer(args);
            }
        }

        public static async Task StartServer(string[] args)
        {
            _cts = new CancellationTokenSource();
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await TryMigrateDbAsync(services, scope);
                services.GetService<IHostApplicationLifetime>()?.ApplicationStopping.Register(Restart);
            }

            try
            {
                await host.RunAsync(_cts.Token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((context, configBuiler) =>
                    {
                        configBuiler.AddConfigurations();
                    });
                    webBuilder.UseStaticWebAssets();
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task TryMigrateDbAsync(IServiceProvider services, IServiceScope scope)
        {
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                if (context.Database.IsSqlServer())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                scope.ServiceProvider.GetRequiredService<ILogger<Program>>()
                    .LogError(ex, "Error occurred while migrating/seeding database.");
                throw;
            }
        }
    }
}