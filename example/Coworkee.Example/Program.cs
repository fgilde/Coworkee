using Coworkee.Core.Extensions;
using Coworkee.Core.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Coworkee.Example
{
    /// <summary>
    /// Example console application demonstrating the Coworkee modular framework
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a host builder
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Add Coworkee Core framework with automatic module discovery
                    services.AddCoworkeeCore(
                        typeof(Domain.Modules.DomainModule).Assembly,
                        typeof(Application.Modules.ApplicationModule).Assembly,
                        typeof(Infrastructure.Modules.InfrastructureModule).Assembly
                    );
                })
                .Build();

            // Get the module loader to see what modules were loaded
            var moduleLoader = host.Services.GetRequiredService<ModuleLoader>();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Coworkee Framework Example Application");
            logger.LogInformation("======================================");
            logger.LogInformation("Loaded Modules:");
            
            foreach (var module in moduleLoader.LoadedModules)
            {
                logger.LogInformation("- {ModuleName} (Dependencies: {Dependencies})", 
                    module.ModuleName, 
                    string.Join(", ", module.Dependencies.Select(d => d.Name)));
            }

            logger.LogInformation("Framework loaded successfully!");
            
            await host.RunAsync();
        }
    }
}