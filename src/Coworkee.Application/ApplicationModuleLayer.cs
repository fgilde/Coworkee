using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coworkee.Application.Configurations;
using Coworkee.Application.Contracts;
using Coworkee.Application.Features.ExtendedAttributes.Commands.AddEdit;
using Coworkee.Application.Features.ExtendedAttributes.Commands.Delete;
using Coworkee.Application.Features.ExtendedAttributes.Queries.Export;
using Coworkee.Application.Features.ExtendedAttributes.Queries.GetAll;
using Coworkee.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using Coworkee.Application.Features.ExtendedAttributes.Queries.GetById;
using Coworkee.Core;
using Coworkee.Domain.Contracts;
using Coworkee.Shared.Wrapper;
using FluentValidation;
using HashidsNet;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nextended.Core.Extensions;

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
            var config = configuration.BindTo<ServerConfiguration>();
            
            services.AddIdHashing(config);
            services.TryAddScoped<ISessionProvider, SimpleSessionProvider>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            services.AddAllWithRegisterAttribute(Assembly.GetExecutingAssembly());
            
            // Add extended attributes handlers
            services.AddExtendedAttributesHandlers();
        }

        private static void AddIdHashing(IServiceCollection services, ServerConfiguration config)
        {
            services.AddSingleton(_ => new Hashids(config.AppConfiguration.IdHashing.Salt, config.AppConfiguration.IdHashing.MinLength));
            ClassMappingConfiguration.RegisterConverters(config.AppConfiguration.IdHashing);
        }

        private static IServiceCollection AddAllWithRegisterAttribute(IServiceCollection services, Assembly locatedInAssembly)
        {
            return services.RegisterAllWithRegisterAsAttribute(locatedInAssembly);
        }

        private static void AddExtendedAttributesHandlers(IServiceCollection services)
        {
            var extendedAttributeTypes = typeof(IEntity)
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true)
                .Select(t => new
                {
                    BaseGenericType = t.BaseType,
                    CurrentType = t
                })
                .Where(t => t.BaseGenericType?.GetGenericTypeDefinition() == typeof(AuditableEntityExtendedAttribute<,,>))
                .ToList();

            foreach (var extendedAttributeType in extendedAttributeTypes)
            {
                var extendedAttributeTypeGenericArguments = extendedAttributeType.BaseGenericType.GetGenericArguments().ToList();
                extendedAttributeTypeGenericArguments.Add(extendedAttributeType.CurrentType);

                RegisterExtendedAttributeHandlers(services, extendedAttributeTypeGenericArguments);
            }
        }

        private static void RegisterExtendedAttributeHandlers(IServiceCollection services, List<System.Type> extendedAttributeTypeGenericArguments)
        {
            #region AddEditExtendedAttributeCommandHandler

            var tRequest = typeof(AddEditExtendedAttributeCommand<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            var tResponse = typeof(Result<>).MakeGenericType(extendedAttributeTypeGenericArguments.First());
            var serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            var implementationType = typeof(AddEditExtendedAttributeCommandHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion

            #region DeleteExtendedAttributeCommandHandler

            tRequest = typeof(DeleteExtendedAttributeCommand<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            tResponse = typeof(Result<>).MakeGenericType(extendedAttributeTypeGenericArguments.First());
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType = typeof(DeleteExtendedAttributeCommandHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion

            #region GetAllExtendedAttributesByEntityIdQueryHandler

            tRequest = typeof(GetAllExtendedAttributesByEntityIdQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            tResponse = typeof(Result<>).MakeGenericType(typeof(List<>).MakeGenericType(
                typeof(GetAllExtendedAttributesByEntityIdResponse<,>).MakeGenericType(
                    extendedAttributeTypeGenericArguments[0], extendedAttributeTypeGenericArguments[1])));
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType = typeof(GetAllExtendedAttributesByEntityIdQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion

            #region GetExtendedAttributeByIdQueryHandler

            tRequest = typeof(GetExtendedAttributeByIdQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            tResponse = typeof(Result<>).MakeGenericType(
                typeof(GetExtendedAttributeByIdResponse<,>).MakeGenericType(
                    extendedAttributeTypeGenericArguments[0], extendedAttributeTypeGenericArguments[1]));
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType = typeof(GetExtendedAttributeByIdQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion

            #region GetAllExtendedAttributesQueryHandler

            tRequest = typeof(GetAllExtendedAttributesQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            tResponse = typeof(Result<>).MakeGenericType(typeof(List<>).MakeGenericType(
                typeof(GetAllExtendedAttributesResponse<,>).MakeGenericType(
                    extendedAttributeTypeGenericArguments[0], extendedAttributeTypeGenericArguments[1])));
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType = typeof(GetAllExtendedAttributesQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion

            #region ExportExtendedAttributesQueryHandler

            tRequest = typeof(ExportExtendedAttributesQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            tResponse = typeof(Result<>).MakeGenericType(typeof(string));
            serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
            implementationType = typeof(ExportExtendedAttributesQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
            services.AddScoped(serviceType, implementationType);

            #endregion
        }
    }
}