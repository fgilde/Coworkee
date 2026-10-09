using Coworkee.AspNetCore;
using Coworkee.AuthServer;
using Coworkee.Core.Modularity;
using Coworkee.Settings;
using MyApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Configuration.AddCoworkeeDatabaseConfiguration(MyAppInfrastructureModule.ConnectionStringName);
builder.AddCoworkee<MyAppAuthModule>();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseCoworkee();
app.Run();

// registration documents go to the documents of the app; administrators get a notification about accounts to activate
[DependsOn(typeof(MyAppInfrastructureModule), typeof(CoworkeeAuthServerModule), typeof(MyApp.Documents.Registration.MyAppRegistrationDocumentsModule),
    typeof(Coworkee.Notifications.CoworkeeNotificationsModule))]
internal sealed class MyAppAuthModule : CoworkeeModule;
