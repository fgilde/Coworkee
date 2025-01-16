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
using Coworkee.Shared.Constants;
using Nextended.Core.Helper;
using Microsoft.JSInterop;

namespace Coworkee.Client
{
    public static class Program
    {        
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                          .CreateDefault(args)
                          .DiscoverBackendUrlIfNeeded();

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

                var jsRuntime = host.Services.GetRequiredService<IJSRuntime>();
                var config = host.Services.GetRequiredService<ClientApplicationConfiguration>();
                var ns = config.JsMainNamespace;
                var res = await new JsStringBuilder(false, ns)
                    .Append(typeof(ApplicationConstants))
                    .Append(typeof(CustomIcons))
                    .ToJsonAsync();

                await jsRuntime.InvokeVoidAsync("___initialLoad", config, res);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}