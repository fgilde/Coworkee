using Coworkee.Client.Blazor;
using Coworkee.Client.Blazor.Navigation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyApp.Web.Client.Api;
using MyApp.Web.Client.Navigation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
builder.Services.AddCoworkeeClient(baseAddress, options =>
{
    options.AppTitle = "MyApp";
    options.AppDescription = "Clean Architecture starter for Blazor, built on Coworkee modules.";
    options.AboutLinks.Add(new AboutLink("Project page", "https://github.com/fgilde/CleanArchitectureBaseBlazor"));
    options.AboutLinks.Add(new AboutLink("Documentation", "https://fgilde.github.io/Coworkee/"));
});
builder.Services.AddHttpClient<ICatalogApi, CatalogApi>(client => client.BaseAddress = baseAddress);
builder.Services.AddHttpClient<IDocumentsApi, DocumentsApi>(client =>
{
    client.BaseAddress = baseAddress;
    client.Timeout = Timeout.InfiniteTimeSpan;
});
builder.Services.AddSingleton<INavigationContributor, MyAppNavigation>();
builder.Services.Configure<NavigationMenuOptions>(MyAppNavigation.Order);
var host = builder.Build();
await host.Services.InitializeCoworkeeClientAsync();
await host.RunAsync();
