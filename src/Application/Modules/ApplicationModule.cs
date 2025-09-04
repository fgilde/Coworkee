using System;
using System.Reflection;
using Coworkee.Application.Configurations;
using Coworkee.Core.Modules;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace Coworkee.Application.Modules
{
    /// <summary>
    /// Application layer module
    /// </summary>
    public class ApplicationModule : ModuleLayer
    {
        private readonly ServerConfiguration _configuration;

        public ApplicationModule(ServerConfiguration configuration = null)
        {
            _configuration = configuration;
        }

        public override string ModuleName => "Application";

        public override Type[] Dependencies => new[] { typeof(Domain.Modules.DomainModule) };

        public override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            if (_configuration != null)
            {
                // Use existing DependencyInjection logic
                return services.AddApplication(_configuration);
            }
            
            // If no configuration provided, register services without configuration-dependent items
            return RegisterCoreApplicationServices(services);
        }

        private IServiceCollection RegisterCoreApplicationServices(IServiceCollection services)
        {
            // Register core application services that don't require configuration
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddAllWithRegisterAttribute(typeof(DependencyInjection).Assembly);
            
            return services;
        }
    }
}