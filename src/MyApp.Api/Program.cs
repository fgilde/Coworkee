using Coworkee.AspNetCore;
using Coworkee.Settings;
using MyApp.Api;
using MyApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Configuration.AddCoworkeeDatabaseConfiguration(MyAppInfrastructureModule.ConnectionStringName);
builder.AddCoworkee<MyAppApiModule>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseCoworkee();
app.Run();

public partial class Program;
