using Coworkee.Shared.Constants.Application;
using Microsoft.Extensions.Configuration;

namespace Coworkee.Server.Extensions
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
            // Here we can add custom config providers like db, or api call based or more json files (builder.AddJsonFile())
            builder.AddEnvironmentVariables();
            builder.AddJsonFile($"{ApplicationConstants.FileAccess.OverridingSettingsFile}", optional: true, reloadOnChange: true);
            return builder;
        }
    }
}