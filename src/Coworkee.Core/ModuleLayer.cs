using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Core
{
    /// <summary>
    /// Interface for module layer entrance points to provide a consistent way for modules to configure services and initialize.
    /// </summary>
    public interface IModuleLayerEntrance
    {
        /// <summary>
        /// Configure services during service registration phase.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The configuration instance.</param>
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// Configure module during application startup phase.
        /// </summary>
        /// <param name="serviceProvider">The configured service provider.</param>
        /// <param name="configuration">The configuration instance.</param>
        void Configure(IServiceProvider serviceProvider, IConfiguration configuration);
    }

    /// <summary>
    /// Base implementation of module layer entrance for Coworkee core functionality.
    /// </summary>
    public class ModuleLayer : IModuleLayerEntrance
    {
        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Core services configuration - can be overridden by derived classes
        }

        /// <inheritdoc />
        public virtual void Configure(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            // Core configuration - can be overridden by derived classes
        }
    }
}
