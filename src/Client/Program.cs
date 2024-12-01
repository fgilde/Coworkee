using System;
using Coworkee.Client.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.Tasks;
using Coworkee.Client.Configuration;
using Coworkee.Client.Managers.Preferences;
using Coworkee.SDK;
using Coworkee.Shared.Constants.Application;

namespace Coworkee.Client
{
    public static class Program
    {        
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                          .CreateDefault(args);

            //builder.Services.AddServiceDefaults();

            builder.AddRootComponents()
                .AddClientServices();

            var host = builder.Build().MakeStaticAccessible();

            var storageService = host.Services.GetRequiredService<ClientPreferenceManager>();
            if (storageService != null)
            {
                CultureInfo culture;
                if (await storageService.GetPreference() is ClientPreference preference)
                    culture = new CultureInfo(preference.LanguageCode);
                else
                    culture = new CultureInfo(ApplicationConstants.DefaultLanguageCode);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
            
            await RegisterServerConfiguration(host);
            await builder.Build().RunAsync();
        }

        private static async Task RegisterServerConfiguration(WebAssemblyHost host)
        {
            try
            {
                var serverConfig = await host.Services.GetRequiredService<IApplicationClient>().System_GetConfigurationAsync();
                host.Services.GetRequiredService<ClientApplicationConfiguration>().ServerConfiguration = serverConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}