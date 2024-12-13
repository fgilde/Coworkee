using Coworkee.Server.Extensions;
using Coworkee.Server.Middlewares;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Coworkee.Application;
using Coworkee.Infrastructure;
using Coworkee.Server.Filters;
using Coworkee.Server.Managers.Preferences;
using Coworkee.Shared.Constants.Application;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Localization;
using Coworkee.Shared;
using Coworkee.Application.Configurations;
using Hangfire.PostgreSql;
using Coworkee.Shared.Helper;

namespace Coworkee.Server
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
            services.AddSignalRServices(serverConfig);
            services.AddCurrentUserServiceAndSession();
            services.AddAllWithRegisterAttribute(typeof(Startup).Assembly);
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddHealthChecks();
            services.AddSerialization();
            services.AddScoped<ServerPreferenceManager>();
            services.AddServerLocalization();
            services.AddYamlLocalizationWithFallback();
            services.AddIdentity();
            services.IfNotNSwag().AddJwtAuthentication(serverConfig);
            services.AddApplication(serverConfig);
            services.AddInfrastructure();
            services.AddApiVersions();
            services.AddOpenApiDocumentation(_configuration);
            services.AddDatabase(_configuration);
            services.AddHangfire((sp,x) =>
            {
                var connectionStr = _configuration.GetConnectionString(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection));
                if(DatabaseProviderDetector.DetectProvider(connectionStr) == DatabaseProvider.Postgres)
                    x.UsePostgreSqlStorage(connectionStr);
                else
                    x.UseSqlServerStorage(connectionStr);
            });
            services.AddHangfireServer();
            services.AddGrpc();
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
            services.AddGptAssistant(_configuration);
            services.AddOptions<BackupOptions>()
                .Bind(_configuration.GetSection(nameof(BackupOptions)))
                .ValidateOnStart()
                .ValidateDataAnnotations();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IStringLocalizer<Startup> localizer,
            IDashboardAuthorizationFilter authorizationFilter)
        {
            #region For Keycloak
            
            var seeder = new KeycloakSeeder();
            System.Threading.Tasks.Task.Delay(15000).ContinueWith(task =>
            {
                _ = seeder.CreateClientAsync();
            });

            #endregion

            app.UseSessionId();
            app.UseCors();
            
            app.UseExceptionHandling(env);
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ApplicationConstants.FileAccess.StaticFileDirectoryName)),
                RequestPath = new PathString($"/{ApplicationConstants.FileAccess.StaticFileDirectoryName}")
            });
            app.UseRequestLocalizationByCulture();
            app.UseRouting();

            app.UseGrpcWeb();
            if (!ServerUtils.ClientRunsOnServer)
                app.UseAuthenticationFromQuery();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard(ApplicationConstants.Routes.Dashboard, new DashboardOptions
            {
                AppPath = !ServerUtils.ClientRunsOnServer ? _configuration["ClientUrl"] : "/",
                DashboardTitle = localizer["{0} Jobs", ApplicationConstants.ApplicationName],
                Authorization = new[] { authorizationFilter }
            });
            app.UseApplicationEndpoints();
            app.UseSwaggerAuthorized();
            app.UseSwagger();



            app.Initialize(_configuration);
        }
    }
   
}