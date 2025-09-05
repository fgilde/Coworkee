using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coworkee.Core;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Application
{
    /// <summary>
    /// Module entrance for Coworkee Application library.
    /// </summary>
    public class ApplicationModuleLayer : ModuleLayer
    {
        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            // Add core application services
            AddCoreApplicationServices(services, configuration);
        }

        /// <inheritdoc />
        public override void Configure(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            base.Configure(serviceProvider, configuration);
        }

        /// <summary>
        /// Add core application services that can be extended by application projects.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        protected virtual void AddCoreApplicationServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register MediatR for CQRS pattern
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Additional core services can be added here
        }
    }
}