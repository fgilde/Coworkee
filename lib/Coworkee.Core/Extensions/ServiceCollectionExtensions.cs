using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using lib.Coworkee.Core.Providers;

namespace lib.Coworkee.Core.Extensions
{
    /// <summary>
    /// Extension methods for configuring Coworkee modules.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a module layer to the service collection.
        /// </summary>
        /// <typeparam name="TModule">The module type implementing IModuleLayerEntrance.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddCoworkeeModule<TModule>(this IServiceCollection services, IConfiguration configuration)
            where TModule : class, IModuleLayerEntrance, new()
        {
            var module = new TModule();
            module.ConfigureServices(services, configuration);
            
            // Register the module instance for later configuration
            services.AddSingleton<IModuleLayerEntrance>(module);
            
            return services;
        }

        /// <summary>
        /// Adds a permission provider to the service collection.
        /// </summary>
        /// <typeparam name="TProvider">The permission provider type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddPermissionProvider<TProvider>(this IServiceCollection services)
            where TProvider : class, IPermissionProvider
        {
            services.AddScoped<IPermissionProvider, TProvider>();
            return services;
        }

        /// <summary>
        /// Adds a role provider to the service collection.
        /// </summary>
        /// <typeparam name="TProvider">The role provider type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddRoleProvider<TProvider>(this IServiceCollection services)
            where TProvider : class, IRoleProvider
        {
            services.AddScoped<IRoleProvider, TProvider>();
            return services;
        }

        /// <summary>
        /// Gets all registered permissions from all permission providers.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>Collection of all permissions.</returns>
        public static IEnumerable<string> GetAllPermissions(this IServiceProvider serviceProvider)
        {
            var providers = serviceProvider.GetServices<IPermissionProvider>();
            return providers.SelectMany(p => p.GetPermissions()).Distinct();
        }

        /// <summary>
        /// Gets all registered roles from all role providers.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>Collection of all roles.</returns>
        public static IEnumerable<string> GetAllRoles(this IServiceProvider serviceProvider)
        {
            var providers = serviceProvider.GetServices<IRoleProvider>();
            return providers.SelectMany(p => p.GetRoles()).Distinct();
        }
    }
}