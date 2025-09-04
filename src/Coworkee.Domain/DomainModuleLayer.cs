using System;
using Coworkee.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Domain
{
    /// <summary>
    /// Module entrance for Coworkee Domain library.
    /// </summary>
    public class DomainModuleLayer : ModuleLayer
    {
        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            // Register domain-specific services here
            // This can be extended by application projects
        }

        /// <inheritdoc />
        public override void Configure(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            base.Configure(serviceProvider, configuration);

            // Configure domain-specific initialization here
        }
    }
}