using CleanArchitectureBase.Application.Extensions;
using CleanArchitectureBase.Infrastructure.Extensions;
using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Server.Middlewares;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using CleanArchitectureBase.Server.Filters;
using CleanArchitectureBase.Server.Managers.Preferences;
using CleanArchitectureBase.Shared.Constants.Application;
using Hangfire.Dashboard;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDashboardAuthorizationFilter, HangfireAuthorizationFilter>();
            services.AddCors();
            services.AddSignalR();
            services.AddCurrentUserServiceAndSession();

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            
            services.AddSerialization();
            services.AddDatabase(_configuration);
            services.AddServerStorage(); //TODO - should implement ServerStorageProvider to work correctly!
            services.AddScoped<ServerPreferenceManager>();
            services.AddServerLocalization();
            services.AddIdentity();
            services.AddJwtAuthentication(services.GetApplicationSettings(_configuration));
            services.AddApplicationLayer();
            services.AddApplicationServices();
            services.AddRepositories();
            services.AddExtendedAttributesUnitOfWork();
            services.AddSharedInfrastructure(_configuration);
            services.AddApiVersions();
            services.AddOpenApiDocumentation(_configuration);
            services.AddHangfire(x => x.UseSqlServerStorage(_configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            services.AddControllers().AddValidators();
            services.AddExtendedAttributesValidators();
            services.AddExtendedAttributesHandlers();
            services.AddRazorPages();
            services.AddLazyCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            IStringLocalizer<Startup> localizer, 
            IWebHostEnvironment environment,
            IDashboardAuthorizationFilter authorizationFilter)
        {
            app.UseSessionId();
            app.UseCors();
            app.UseExceptionHandling(env);
            app.UseHttpsRedirection();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
                RequestPath = new PathString("/Files")
            });
            app.UseRequestLocalizationByCulture();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard(ApplicationConstants.Hangfire.DashboardRoute, new DashboardOptions
            {
                DashboardTitle = localizer["{0} Jobs", ApplicationConstants.ApplicationName],
                AppPath = "https://localhost:5001",
                Authorization = new[] { authorizationFilter }
            });
            app.UseEndpoints();
            app.UseSwagger();
            app.Initialize(_configuration);
        }
    }
}