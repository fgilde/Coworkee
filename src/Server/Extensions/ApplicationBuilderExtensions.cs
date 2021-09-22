using System;
using System.Globalization;
using System.Linq;
using CleanArchitectureBase.Application.Hubs;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Server.Middlewares;
using CleanArchitectureBase.Shared.Constants.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureBase.Server.Extensions
{
    internal static class ApplicationBuilderExtensions
    {

        public static IApplicationBuilder UseSessionId(this IApplicationBuilder app)
        {
            app.UseSession();
            app.Use(async (context, next) =>
            {
                var id = context.Session.GetString(ApplicationConstants.SessionIdKey);
                if (string.IsNullOrEmpty(id))
                    context.Session.SetString(ApplicationConstants.SessionIdKey, Guid.NewGuid().ToString());

                await next.Invoke();
            });
            return app;
        }

        internal static IApplicationBuilder UseExceptionHandling(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }

            return app;
        }

        internal static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
        {
            app.UseSwaggerUi3(a => {
                a.OperationsSorter = "alpha";
                a.TagsSorter = "alpha";
            });
            app.UseOpenApi(options =>
            {
                options.PostProcess = (document, request) =>
                {
                    // Patch server URL for Swagger UI
                    var prefix = $"/api/v" + document.Info.Version.Split('.')[0];
                    document.Servers.First().Url += prefix;
                };
            });
            return app;
        }

        internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
            => app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHub<SignalRHub>(ApplicationConstants.SignalR.HubUrl);
                endpoints.MapHub<ClientEventHub>(ApplicationConstants.SignalR.EventHubUrl);

            });

        internal static IApplicationBuilder UseRequestLocalizationByCulture(this IApplicationBuilder app)
        {
            var supportedCultures = LocalizationConstants.SupportedLanguages.Select(l => new CultureInfo(l.Code)).ToArray();
            app.UseRequestLocalization(options =>
            {
                options.SupportedUICultures = supportedCultures;
                options.SupportedCultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
                options.ApplyCurrentCultureToResponseHeaders = true;
            });

            app.UseMiddleware<RequestCultureMiddleware>();

            return app;
        }

        internal static IApplicationBuilder Initialize(this IApplicationBuilder app, Microsoft.Extensions.Configuration.IConfiguration _configuration)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }

            return app;
        }
    }
}