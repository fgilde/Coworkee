using Coworkee.Workflows.Elsa.Studio;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Elsa Studio, signed in at the auth server like the web app (a client of its own); the designer works with the Elsa API of MyApp.Api
builder.AddCoworkeeElsaStudio();

var app = builder.Build();
app.MapDefaultEndpoints();
app.MapCoworkeeElsaStudio();
app.Run();
