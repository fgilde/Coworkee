using System.IO;
using Coworkee.Application;
using Coworkee.Application.Configurations;
using Coworkee.Infrastructure;
using Coworkee.Infrastructure.Contexts;
using Coworkee.Server.Extensions;
using Coworkee.Server.Filters;
using Coworkee.Server.Managers.Preferences;
using Coworkee.Server.Middlewares;
using Coworkee.Shared;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Helper;
using Delta;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;

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
            services.AddSignalRServices(_configuration);
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
            services.AddJwtAuthentication(serverConfig);
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
                    x.UseSqlServerStorage(connectionStr, new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = true
                    });
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
            services.AddAIAssistant(_configuration);
            services.AddOptions<BackupOptions>()
                .Bind(_configuration.GetSection(nameof(BackupOptions)))
                .ValidateOnStart()
                .ValidateDataAnnotations();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IStringLocalizer<Startup> localizer,
            IDashboardAuthorizationFilter authorizationFilter)
        {
            //#region For Keycloak

            //var config = ServerConfiguration.Instance;
            //if (config.PublicSettings.KeycloakEnabled)
            //{
            //    config.PublicSettings.Endpoints.TryGetValue(ApplicationConstants.ServiceNames.Keycloak, out string keycloakUrl);
            //    var seeder = new KeycloakSeeder(keycloakUrl);
            //    System.Threading.Tasks.Task.Delay(15000).ContinueWith(task =>
            //    {
            //        _ = seeder.CreateClientAsync();
            //    });
            //}

            //#endregion

            app.UseSessionId();
            app.UseCors();
            
            app.UseExceptionHandling(env);
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            var staticFilePath = Path.Combine(env.WebRootPath, "..", ApplicationConstants.FileAccess.StaticFileDirectoryName);
            if (!Directory.Exists(staticFilePath))
                Directory.CreateDirectory(staticFilePath);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(staticFilePath),
                RequestPath = new PathString($"/{ApplicationConstants.FileAccess.StaticFileDirectoryName}")
            });

            app.UseRequestLocalizationByCulture();
            app.UseRouting();

            app.UseGrpcWeb();
            if (!ApplicationConstants.HostClientInServer)
                app.UseAuthenticationFromQuery();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDelta<ApplicationDbContext>();


            app.UseHangfireDashboard(ApplicationConstants.Routes.Dashboard, new DashboardOptions
            {
                AppPath = !ApplicationConstants.HostClientInServer ? _configuration["ClientUrl"] : "/",
                DashboardTitle = localizer["{0} Jobs", ApplicationConstants.ApplicationName],
                Authorization = [authorizationFilter]
            });
            app.UseApplicationEndpoints();
            app.UseSwaggerAuthorized();
            app.UseSwagger();

            
            app.Initialize(_configuration);
        }
    }
   
}