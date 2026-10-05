using Coworkee.Application.Messaging;
using Coworkee.AspNetCore;
using Coworkee.AspNetCore.Http;
using Coworkee.Core.Modularity;
using Coworkee.Infrastructure.Outbox;
using Coworkee.ResponseFilters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyApp.Application.System;
using MyApp.Catalog;
using MyApp.Infrastructure;

namespace MyApp.Api;

[DependsOn(typeof(MyAppDatabaseModule))]
public sealed class MyAppApiModule : CoworkeeModule, IWebModule
{
    public const string Audience = "myapp_api";

    public override void ConfigureServices(ModuleServiceContext context)
    {
        context.Services.AddCoworkeeOutboxProcessing<MyAppDbContext>();
        context.Services.AddCoworkeeResponseFilters([typeof(MyAppCatalogModule).Assembly]);
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var authority = context.Configuration["Auth:Authority"];
                options.Authority = authority;
                options.Audience = Audience;
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = authority?.StartsWith("http://", StringComparison.OrdinalIgnoreCase) != true;
            });
    }

    public void ConfigureApplication(WebApplication app)
    {
        var system = app.MapGroup("/api/v1/system").WithTags("System");
        system.MapGet("/info", (IDispatcher dispatcher, CancellationToken ct) => dispatcher.SendAsync(new GetSystemInfo(), ct).ToHttpResult())
            .WithName("GetSystemInfo");
    }
}
