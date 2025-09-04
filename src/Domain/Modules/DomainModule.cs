using Coworkee.Core.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Domain.Modules
{
    /// <summary>
    /// Domain layer module
    /// </summary>
    public class DomainModule : ModuleLayer
    {
        public override string ModuleName => "Domain";

        public override Type[] Dependencies => new Type[0]; // No dependencies - domain is the core layer

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Domain layer services registration
            // This layer typically doesn't have many DI registrations as it contains mainly entities and interfaces
            return services;
        }
    }
}