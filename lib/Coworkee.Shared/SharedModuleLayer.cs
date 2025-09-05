using System;
using Coworkee.Core;
using Coworkee.Core.Extensions;
using Coworkee.Shared.Constants.Permission;
using Coworkee.Shared.Constants.Role;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Shared
{
    /// <summary>
    /// Module entrance for Coworkee Shared library.
    /// </summary>
    public class SharedModuleLayer : ModuleLayer
    {
        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            // Register core permission and role providers
            services.AddPermissionProvider<CorePermissionProvider>();
            services.AddRoleProvider<CoreRoleProvider>();

            // Register other shared services here
        }

        /// <inheritdoc />
        public override void Configure(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            base.Configure(serviceProvider, configuration);

            // Configure shared initialization here
        }
    }
}