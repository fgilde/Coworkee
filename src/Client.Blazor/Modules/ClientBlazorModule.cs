using Coworkee.Core.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Client.Blazor.Modules
{
    /// <summary>
    /// Client Blazor module
    /// </summary>
    public class ClientBlazorModule : ModuleLayer
    {
        public override string ModuleName => "Client.Blazor";

        public override Type[] Dependencies => new[] { typeof(Infrastructure.Modules.InfrastructureModule) };

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Client-specific services registration
            // This would include Blazor-specific services, HTTP clients, etc.
            return services;
        }
    }
}