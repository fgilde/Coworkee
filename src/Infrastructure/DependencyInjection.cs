using System;
using System.Linq;
using Coworkee.Application;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Serialization.Serializers;
using Coworkee.Application.Contracts.Services.Storage;
using Coworkee.Application.Contracts.Services.Storage.Provider;
using Coworkee.Application.Serialization.JsonConverters;
using Coworkee.Application.Serialization.Options;
using Coworkee.Application.Serialization.Serializers;
using Coworkee.Infrastructure.Repositories;
using Coworkee.Infrastructure.Services.Storage;
using Coworkee.Infrastructure.Services.Storage.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace Coworkee.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            return services
                .AddAllWithRegisterAttribute(typeof(DependencyInjection).Assembly)
                .AddRepositories()
                .AddServerStorage() //TODO - should implement ServerStorageProvider to work correctly!
                .AddExtendedAttributesUnitOfWork();
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<IBrandRepository, BrandRepository>()
                .AddTransient<IDocumentRepository, DocumentRepository>()
                .AddTransient<IDocumentTypeRepository, DocumentTypeRepository>()
                .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }

        private static IServiceCollection AddExtendedAttributesUnitOfWork(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IExtendedAttributeUnitOfWork<,,>), typeof(ExtendedAttributeUnitOfWork<,,>));
        }

        private static IServiceCollection AddServerStorage(this IServiceCollection services)
            => AddServerStorage(services, null);

        private static IServiceCollection AddServerStorage(this IServiceCollection services, Action<SystemTextJsonOptions> configure)
        {
            return services
                .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
                .AddScoped<IStorageProvider, ServerStorageProvider>()
                .AddScoped<IServerStorageService, ServerStorageService>()
                .AddScoped<ISyncServerStorageService, ServerStorageService>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    if (configureOptions.JsonSerializerOptions.Converters.All(c => c.GetType() != typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
        }
    }
}