using Coworkee.AspNetCore;
using MyApp.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddCoworkee<MyAppApiModule>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseCoworkee();
app.Run();

public partial class Program;
