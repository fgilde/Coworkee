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
using CleanArchitectureBase.Application;
using CleanArchitectureBase.Infrastructure;
using CleanArchitectureBase.Server.Filters;
using CleanArchitectureBase.Server.Managers.Preferences;
using CleanArchitectureBase.Shared.Constants.Application;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.OData;
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
            var serverConfig = services.AddApplicationSettings(_configuration);
            services.AddTransient<IDashboardAuthorizationFilter, HangfireAuthorizationFilter>();
            services.AddCors(options => options.AddDefaultPolicy(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
            services.AddSignalR().AddAzureSignalR();
            services.AddCurrentUserServiceAndSession();
            services.AddAllWithRegisterAttribute(typeof(Startup).Assembly);
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddHealthChecks();
            services.AddSerialization();
            services.AddDatabase(_configuration);
            services.AddScoped<ServerPreferenceManager>();
            services.AddServerLocalization();
            services.AddIdentity();
            services.AddJwtAuthentication(serverConfig);
            services.AddApplication();
            services.AddInfrastructure();
            services.AddApiVersions();
            services.AddOpenApiDocumentation(_configuration);
            services.AddHangfire(x => x.UseSqlServerStorage(_configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();
            
            services.AddControllersWithViews(options =>
                {
                    options.Filters.Add<ApiExceptionFilterAttribute>();
                    options.ModelBinderProviders.Insert(0, new TransferableExpressionModelBinderProvider());
                })
                .AddValidators()
                .AddXmlDataContractSerializerFormatters()
                .AddOData(options =>
                {
                    options.EnableQueryFeatures().SetMaxTop(1000);
                    //options.AddRouteComponents("odata", modelBuilder.GetEdmModel());
                });
            //.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); // TODO: Currently not working with extended attributes (EntityExtendedAttributeType) and all other enums inherit from byte
            services.AddExtendedAttributesValidators();
            services.AddExtendedAttributesHandlers();
            services.AddRazorPages();
            services.AddLazyCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            IStringLocalizer<Startup> localizer,
            IDashboardAuthorizationFilter authorizationFilter)
        {
            app.UseSessionId();
            app.UseCors();
            app.UseHealthChecks("/health");
            app.UseExceptionHandling(env);
            app.UseHttpsRedirection();
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
                Authorization = new[] { authorizationFilter }
            });
            app.UseEndpoints();
            app.UseSwaggerAuthorized();
            app.UseSwagger();
            app.Initialize(_configuration);
        }
    }
    
}