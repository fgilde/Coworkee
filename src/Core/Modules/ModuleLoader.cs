using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coworkee.Core.Modules
{
    /// <summary>
    /// Module loader for discovering and registering modules
    /// </summary>
    public class ModuleLoader
    {
        private readonly ILogger<ModuleLoader> _logger;
        private readonly List<IModuleLayerEntrance> _loadedModules = new();

        public ModuleLoader(ILogger<ModuleLoader> logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Load modules from assemblies
        /// </summary>
        /// <param name="assemblies">Assemblies to scan for modules</param>
        /// <returns>Discovered modules</returns>
        public List<IModuleLayerEntrance> LoadModulesFromAssemblies(params Assembly[] assemblies)
        {
            var modules = new List<IModuleLayerEntrance>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var moduleTypes = assembly.GetTypes()
                        .Where(t => typeof(IModuleLayerEntrance).IsAssignableFrom(t) && 
                                   !t.IsInterface && 
                                   !t.IsAbstract)
                        .ToList();

                    foreach (var moduleType in moduleTypes)
                    {
                        try
                        {
                            var module = (IModuleLayerEntrance)Activator.CreateInstance(moduleType);
                            modules.Add(module);
                            _logger?.LogInformation("Loaded module: {ModuleName} from {Assembly}", 
                                module.ModuleName, assembly.GetName().Name);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Failed to create instance of module {ModuleType}", moduleType.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to scan assembly {Assembly} for modules", assembly.GetName().Name);
                }
            }

            return modules;
        }

        /// <summary>
        /// Configure services for all loaded modules, respecting dependencies
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="modules">Modules to configure</param>
        /// <returns>Configured service collection</returns>
        public IServiceCollection ConfigureModules(IServiceCollection services, List<IModuleLayerEntrance> modules)
        {
            var orderedModules = OrderModulesByDependencies(modules);
            
            foreach (var module in orderedModules)
            {
                try
                {
                    _logger?.LogInformation("Configuring module: {ModuleName}", module.ModuleName);
                    module.ConfigureServices(services);
                    _loadedModules.Add(module);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Failed to configure module {ModuleName}", module.ModuleName);
                    throw;
                }
            }

            return services;
        }

        /// <summary>
        /// Order modules by their dependencies
        /// </summary>
        private List<IModuleLayerEntrance> OrderModulesByDependencies(List<IModuleLayerEntrance> modules)
        {
            var ordered = new List<IModuleLayerEntrance>();
            var remaining = new List<IModuleLayerEntrance>(modules);
            var moduleTypes = modules.ToDictionary(m => m.GetType(), m => m);

            while (remaining.Any())
            {
                var canAdd = remaining.Where(m => 
                    m.Dependencies.All(dep => ordered.Any(o => dep.IsAssignableFrom(o.GetType()))))
                    .ToList();

                if (!canAdd.Any())
                {
                    // Circular dependency or missing dependency
                    var remainingNames = string.Join(", ", remaining.Select(m => m.ModuleName));
                    _logger?.LogWarning("Potential circular dependency or missing dependency for modules: {Modules}", remainingNames);
                    // Add remaining modules anyway
                    ordered.AddRange(remaining);
                    break;
                }

                ordered.AddRange(canAdd);
                foreach (var module in canAdd)
                {
                    remaining.Remove(module);
                }
            }

            return ordered;
        }

        /// <summary>
        /// Get all loaded modules
        /// </summary>
        public IReadOnlyList<IModuleLayerEntrance> LoadedModules => _loadedModules.AsReadOnly();
    }
}