using Microsoft.Extensions.Configuration;
using Serilog;

namespace CleanArchitectureBase.Server.Extensions
{
    public static class ConfigBuilderExtensions
    {
        public static IConfigurationBuilder AddConfigurations(this IConfigurationBuilder builder)
        {
            // Here we can add custom config providers like db, or api call based or more json files (builder.AddJsonFile())
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Build()).CreateLogger();
            return builder;
        }
    }
}