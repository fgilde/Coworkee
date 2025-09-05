using lib.Coworkee.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace lib.Coworkee.Client.Blazor
{
    /// <summary>
    /// Module entrance for Coworkee Client Blazor library.
    /// </summary>
    public class ClientBlazorModuleLayer : ModuleLayer
    {
        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            // Register core client services here
            // This can be extended by application projects for specific client features
        }

        /// <inheritdoc />
        public override void Configure(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            base.Configure(serviceProvider, configuration);

            // Configure client initialization here
        }
    }
}