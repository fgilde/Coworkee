using System;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Core.Modules
{
    /// <summary>
    /// Base module layer implementation
    /// </summary>
    public abstract class ModuleLayer : IModuleLayerEntrance
    {
        public abstract string ModuleName { get; }
        
        public virtual Type[] Dependencies { get; } = Array.Empty<Type>();

        public virtual IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }
    }
}