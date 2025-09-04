using System;
using Coworkee.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Application
{
    /// <summary>
    /// Custom application module that extends Coworkee Application library.
    /// Demonstrates how to override and extend core functionality.
    /// </summary>
    public class ExampleApplicationModule : ApplicationModuleLayer
    {
        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Call base configuration first to get all core services
            base.ConfigureServices(services, configuration);

            // Add business-specific services here
            // This demonstrates the extension point for business logic
            Console.WriteLine("ExampleApplicationModule: Business-specific services registered");
        }

        /// <inheritdoc />
        public override void Configure(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            base.Configure(serviceProvider, configuration);

            Console.WriteLine("ExampleApplicationModule: Configuration completed");
        }
    }
}