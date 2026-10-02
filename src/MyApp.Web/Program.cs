using Coworkee.Bff;
using MyApp.Web;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddCoworkeeBff();
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapCoworkeeBff();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MyApp.Web.Client._Imports).Assembly, typeof(Coworkee.Client.Blazor.CoworkeeClientOptions).Assembly);
app.Run();
