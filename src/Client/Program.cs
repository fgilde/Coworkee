using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Client.Managers.Preferences;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.Extensions.Configuration;

namespace CleanArchitectureBase.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                          .CreateDefault(args)
                          .AddRootComponents()
                          .AddClientServices();
            var host = builder.Build();
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
            //builder.RootComponents.RegisterAsCustomElement<Inventory>("inventory-grid");
            await builder.Build().RunAsync();
        }
    }
}