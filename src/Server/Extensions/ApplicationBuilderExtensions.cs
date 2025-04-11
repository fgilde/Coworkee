using System;
using System.IO;
using System.Linq;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Hubs;
using Coworkee.Data;
using Coworkee.Server.Middlewares;
using Coworkee.Shared.Constants.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;

namespace Coworkee.Server.Extensions
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
            var wwwrootSwaggerUiHeaderHtml = "wwwroot/swagger-ui/header.html";
            app.UseSwaggerUi(a => {
                a.OperationsSorter = "alpha";
                a.TagsSorter = "alpha";                
                a.CustomHeadContent = File.Exists(wwwrootSwaggerUiHeaderHtml) ? File.ReadAllText(wwwrootSwaggerUiHeaderHtml) : string.Empty;
                a.CustomJavaScriptPath = "/swagger-ui/scripts.js";
                a.CustomStylesheetPath = "/swagger-ui/styles.css";
                a.PersistAuthorization = true;
            });
            app.UseOpenApi(options =>
            {
                options.PostProcess = (document, request) =>
                {
                    ////var cfg = app.ApplicationServices.GetRequiredService<ServerConfiguration>();
                    //var token = request.HttpContext.RequestServices.GetRequiredService<ITokenService>().GenerateTokenForUser(request.HttpContext.RequestServices.GetService<ISessionProvider>()?.UserIdFromSession).Result;
                    //if (!string.IsNullOrEmpty(token))
                    //{
                    //    OpenApiHeader openApiHeader = new OpenApiHeader { Name = "Authorization", Id = "hidden-default-header", Kind = OpenApiParameterKind.Header, Default = $"Bearer {token}" };
                    //    document.Operations.Apply(description => description.Operation.Parameters.Add(openApiHeader));
                    //}
                    // Patch server URL for Swagger UI
                    var prefix = $"/api/v" + document.Info.Version.Split('.')[0];
                    document.Servers.First().Url += prefix;
                };
            });
            return app;
        }

        internal static IApplicationBuilder UseApplicationEndpoints(this IApplicationBuilder app)
            => app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHub<ClientEventHub>(ApplicationConstants.SignalR.EventHubUrl);
                endpoints.MapGrpcService<MainDataService>().EnableGrpcWeb();
            });

        internal static IApplicationBuilder UseRequestLocalizationByCulture(this IApplicationBuilder app)
        {
            var supportedCultures = LocalizationConstants.ExistingTranslations.Select(l => l.ToCulture()).ToArray();
            app.UseRequestLocalization(options =>
            {
                options.SupportedUICultures = supportedCultures;
                options.SupportedCultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture(supportedCultures.FirstOrDefault(c => c.Name == ApplicationConstants.DefaultLanguageCode) ?? supportedCultures.First());
                options.ApplyCurrentCultureToResponseHeaders = true;
            });

            app.UseMiddleware<RequestCultureMiddleware>();

            return app;
        }

        internal static IApplicationBuilder Initialize(this IApplicationBuilder app, Microsoft.Extensions.Configuration.IConfiguration _configuration)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>().ToArray();
            foreach (var initializer in initializers)
                initializer.Initialize();
            
            return app;
        }
    }
}