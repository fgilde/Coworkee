using Coworkee.Client.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyApp.Web.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
builder.Services.AddCoworkeeClient(baseAddress, options => options.AppTitle = "MyApp");
builder.Services.AddHttpClient<IMyAppApi, MyAppApi>(client =>
{
    client.BaseAddress = baseAddress;
    client.Timeout = Timeout.InfiniteTimeSpan;
});
builder.Services.AddSingleton<INavigationContributor, MyAppNavigation>();
await builder.Build().RunAsync();
