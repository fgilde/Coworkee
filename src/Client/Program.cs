using CleanArchitectureBase.Client.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Client.Managers.Preferences;
using CleanArchitectureBase.Shared.Constants.Application;

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
            await builder.Build().RunAsync();
        }
    }
}