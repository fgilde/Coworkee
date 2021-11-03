using System.Linq;
using System.Reflection;
using AKSoftware.Localization.MultiLanguages;
using CleanArchitectureBase.Shared.Localizers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Shared
{
    public static class Resource
    {
        public static Assembly Assembly = typeof(Resource).Assembly;

        public static IServiceCollection AddYamlLocalizationWithFallback(this IServiceCollection services)
        {
            var descriptor = ServiceDescriptor.Transient(typeof(IStringLocalizer<>), typeof(YamlLocalizer<>));
            var existing = services.FirstOrDefault(d => d.ServiceType == descriptor.ServiceType);
            services.Insert(0, descriptor);
            if (existing != null)
            {
                services.Add(ServiceDescriptor.Transient(typeof(IStringLocalizer<>), typeof(AggregatedLocalizer<>)));
            }
            
            services.AddLanguageContainer(Assembly);
            return services;
        }
    }
}