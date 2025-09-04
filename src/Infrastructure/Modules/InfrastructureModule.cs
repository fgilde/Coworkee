using Coworkee.Core.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Infrastructure.Modules
{
    /// <summary>
    /// Infrastructure layer module
    /// </summary>
    public class InfrastructureModule : ModuleLayer
    {
        public override string ModuleName => "Infrastructure";

        public override Type[] Dependencies => new[] { typeof(Application.Modules.ApplicationModule) };

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Use existing DependencyInjection logic
            return services.AddInfrastructure();
        }
    }
}