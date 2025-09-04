using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Coworkee.Core;
using Coworkee.Core.Extensions;
using Coworkee.Shared;
using Coworkee.Domain;

namespace Example.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Coworkee Modular Framework Example ===");
            Console.WriteLine();

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Example:Name", "Coworkee Example Application"},
                    {"Example:Version", "1.0.0"}
                })
                .Build();

            // Create host builder using .NET hosting model
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    Console.WriteLine("Configuring services with Coworkee modules...");
                    Console.WriteLine();

                    // Register Coworkee modules in dependency order
                    // Each module configures its own services and can be overridden
                    
                    services.AddCoworkeeModule<SharedModuleLayer>(configuration);
                    Console.WriteLine("✓ Shared module configured (Core permissions and roles)");
                    
                    services.AddCoworkeeModule<DomainModuleLayer>(configuration);
                    Console.WriteLine("✓ Domain module configured (Core entities and contracts)");
                    
                    // This is our custom application module that extends core functionality
                    services.AddCoworkeeModule<ExampleApplicationModule>(configuration);
                    Console.WriteLine("✓ Example Application module configured (Business-specific features)");
                })
                .Build();

            Console.WriteLine();
            Console.WriteLine("=== Starting Application ===");
            Console.WriteLine();

            // Configure all modules (this would typically happen during app startup)
            using (var scope = host.Services.CreateScope())
            {
                var modules = scope.ServiceProvider.GetServices<IModuleLayerEntrance>();
                foreach (var module in modules)
                {
                    module.Configure(scope.ServiceProvider, configuration);
                }
            }

            Console.WriteLine();
            Console.WriteLine("=== Application completed successfully! ===");
            Console.WriteLine();
            Console.WriteLine("This demonstrates:");
            Console.WriteLine("1. ✓ Modular architecture with clean separation of concerns");
            Console.WriteLine("2. ✓ Provider pattern for extending permissions and roles");
            Console.WriteLine("3. ✓ Easy module registration and configuration");
            Console.WriteLine("4. ✓ Core vs Business logic separation");
            Console.WriteLine("5. ✓ Extensible and overridable services");
        }
    }
}
