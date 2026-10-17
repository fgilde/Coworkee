using Coworkee.Bff;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// signs in at the auth server like the web app (a client of its own) and forwards /api, /odata and /hubs with the access token
builder.AddCoworkeeBff();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseDefaultFiles();
// the MudEx web components (file previews) bring the .NET runtime, whose ICU data files end in .dat
var contentTypes = new FileExtensionContentTypeProvider();
contentTypes.Mappings[".dat"] = "application/octet-stream";
app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = contentTypes });
app.MapCoworkeeBff();
app.MapFallbackToFile("index.html");
app.Run();
