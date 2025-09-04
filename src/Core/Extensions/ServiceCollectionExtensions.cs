using System;
using System.Reflection;
using Coworkee.Core.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coworkee.Core.Extensions
{
    /// <summary>
    /// Extension methods for service collection to support modular architecture
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Coworkee core services and auto-discover modules
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assemblies">Assemblies to scan for modules</param>
        /// <returns>Service collection with core services and modules configured</returns>
        public static IServiceCollection AddCoworkeeCore(this IServiceCollection services, params Assembly[] assemblies)
        {
            // Add core services
            services.AddLogging();
            services.AddSingleton<ModuleLoader>();

            // Auto-discover and configure modules
            using var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<ModuleLoader>>();
            var moduleLoader = new ModuleLoader(logger);

            // If no assemblies specified, scan calling assembly
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            var modules = moduleLoader.LoadModulesFromAssemblies(assemblies);
            moduleLoader.ConfigureModules(services, modules);

            // Register the module loader as singleton with loaded modules
            services.AddSingleton(moduleLoader);

            return services;
        }

        /// <summary>
        /// Add a specific module to the service collection
        /// </summary>
        /// <typeparam name="TModule">Module type</typeparam>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection with module configured</returns>
        public static IServiceCollection AddModule<TModule>(this IServiceCollection services)
            where TModule : class, IModuleLayerEntrance, new()
        {
            var module = new TModule();
            return module.ConfigureServices(services);
        }

        /// <summary>
        /// Add a specific module instance to the service collection
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="module">Module instance</param>
        /// <returns>Service collection with module configured</returns>
        public static IServiceCollection AddModule(this IServiceCollection services, IModuleLayerEntrance module)
        {
            return module.ConfigureServices(services);
        }
    }
}