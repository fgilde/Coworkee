using System;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Core.Modules
{
    /// <summary>
    /// Interface for module layer entrance. Modules should implement this interface to provide dependency injection registration.
    /// </summary>
    public interface IModuleLayerEntrance
    {
        /// <summary>
        /// Configure services for the module
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Modified service collection</returns>
        IServiceCollection ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Get the module name
        /// </summary>
        string ModuleName { get; }

        /// <summary>
        /// Get the module dependencies - other modules this module depends on
        /// </summary>
        Type[] Dependencies { get; }
    }
}