using Coworkee.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Infrastructure
{
    /// <summary>
    /// Module entrance for Coworkee Infrastructure library.
    /// </summary>
    public class InfrastructureModuleLayer : ModuleLayer
    {
        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            // Register core infrastructure services here
            // This can be extended by application projects for specific data contexts, repositories, etc.
        }

        /// <inheritdoc />
        public override void Configure(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            base.Configure(serviceProvider, configuration);

            // Configure infrastructure initialization here
        }
    }
}