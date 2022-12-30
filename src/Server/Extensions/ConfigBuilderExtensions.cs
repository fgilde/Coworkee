using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.Extensions.Configuration;

namespace CleanArchitectureBase.Server.Extensions
{
    public static class ConfigBuilderExtensions
    {
        public static TResult BindTo<TResult>(this IConfiguration configuration) where TResult : new()
        {
            var result = new TResult();
            configuration.Bind(result);
            return result;
        }

        public static IConfigurationBuilder AddConfigurations(this IConfigurationBuilder builder)
        {
            builder.AddEnvironmentVariables();
            builder.AddJsonFile($"{ApplicationConstants.FileAccess.OverridingSettingsFile}", optional: true, reloadOnChange: true);
            return builder;
        }
    }
}