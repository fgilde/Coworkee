using Coworkee.Application.Configurations;
using Coworkee.Infrastructure;
using Coworkee.Infrastructure.Contexts;
using Coworkee.Infrastructure.Models.Identity;
using Coworkee.Server.Localization;
using Coworkee.Server.Managers.Preferences;
using Coworkee.Server.Permission;
using Coworkee.Server.Services;
using Coworkee.Server.Settings;
using Coworkee.Shared.Constants.Localization;
using Coworkee.Shared.Constants.Permission;
using Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Coworkee.Application.Contracts;
using Coworkee.Application.Contracts.Serialization.Options;
using Coworkee.Application.Contracts.Serialization.Serializers;
using Coworkee.Application.Contracts.Serialization.Settings;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Contracts.Services.Account;
using Coworkee.Application.Serialization.JsonConverters;
using Coworkee.Application.Serialization.Options;
using Coworkee.Application.Serialization.Serializers;
using Coworkee.Application.Serialization.Settings;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using Coworkee.Application.AssistantFeatures;
using GptInvoke;
using OpenAI.Models;
using System.Configuration;

namespace Coworkee.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddGptAssistant(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                var gptConfig = ServerConfiguration.Instance.CognitiveServices.OpenAi;
                var assistantAvailable = ServerConfiguration.Instance.PublicSettings.AssistantAvailable && !string.IsNullOrWhiteSpace(gptConfig.ApiKey);
                configuration[$"{nameof(Publicsettings)}:{nameof(Publicsettings.AssistantAvailable)}"] = assistantAvailable.ToString();
                if (assistantAvailable)
                {
                    services.AddGptActionInvoker(settings =>
                    {
                        settings.ApiKey = gptConfig.ApiKey;
                        settings.Model = string.IsNullOrWhiteSpace(gptConfig.Model) ? Model.GPT4 : new Model(gptConfig.Model);
                    }, typeof(AddProduct).Assembly);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return services;
        }
        internal static IServiceCollection AddSignalRServices(this IServiceCollection services, ServerConfiguration configuration)
        {
            if (configuration.Azure.SignalR.Enabled && !string.IsNullOrEmpty(configuration.Azure.SignalR.ConnectionString))
                services.AddSignalR().AddAzureSignalR();
            else
                services.AddSignalR();
            return services;
        }

        internal static IServiceCollection AddCurrentUserServiceAndSession(this IServiceCollection services)
        {
            services.AddSession();
            services.AddHttpContextAccessor();
            services.RemoveAll<ISessionProvider>().AddTransient<ISessionProvider, SessionProvider>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }

        internal static async Task<IStringLocalizer> GetRegisteredServerLocalizerAsync<T>(this IServiceCollection services) where T : class
        {
            var serviceProvider = services.BuildServiceProvider();
            await SetCultureFromServerPreferenceAsync(serviceProvider);
            var result = serviceProvider.GetService<IStringLocalizer<T>>();
            await serviceProvider.DisposeAsync();
            return result;
        }

        private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
        {
            var storageService = serviceProvider.GetService<ServerPreferenceManager>();
            if (storageService != null)
            {
                // TODO - should implement ServerStorageProvider to work correctly!
                var culture = await storageService.GetPreference() is ServerPreference preference ? new CultureInfo(preference.LanguageCode) : new CultureInfo(LocalizationConstants.DefaultLanguageCode);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
        {
            services.AddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
            return services;
        }

        internal static ServerConfiguration AddApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            ApplicationConstants.HostClientInServer = ServerUtils.ClientRunsOnServer;
            configuration[$"{nameof(Publicsettings)}:{nameof(Publicsettings.HostClientInServer)}"] = ServerUtils.ClientRunsOnServer.ToString();

            ServerConfiguration.Instance = configuration.BindTo<ServerConfiguration>(); // One time to have static instance filled as early as possible
            services.AddTransient(_ => ServerConfiguration.Instance = configuration.BindTo<ServerConfiguration>()); // Important as func to have always updated settings static instance is updated as well on each read
            services.AddTransient(s => s.GetRequiredService<ServerConfiguration>().PublicSettings);
            services.Configure<ServerConfiguration>(configuration);
            return configuration.Get<ServerConfiguration>();
        }

        public static IServiceCollection AddApiVersions(this IServiceCollection services)
        {
            return services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = ApiVersions.Newest;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
                .AddVersionedApiExplorer(options =>
                {
                    options.DefaultApiVersion = ApiVersions.Newest;
                    options.GroupNameFormat = ApiVersions.GroupNameFormat;
                    options.SubstituteApiVersionInUrl = true;
                });
        }

        public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration configuration)
        {
            var configSection = configuration.GetSection("ApiDocumentation");
            foreach (var version in ApiVersions.All.Reverse())
            {
                async void Configure(AspNetCoreOpenApiDocumentGeneratorSettings options)
                {
                    var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                    options.Title = configSection.GetValue<string>(nameof(options.Title));
                    options.Description = configSection.GetValue<string>(nameof(options.Description));
                    options.DocumentName = ApiVersions.DocumentVersionPrefix + version.MajorVersion;
                    options.ApiGroupNames = new[] { ApiVersions.DocumentVersionPrefix + version.MajorVersion };
                    options.Version = ApiVersions.VersionString(version);
                    // Patch document for Azure API Management
                    //options.AllowReferencesWithProperties = true;
                    options.PostProcess = document => configSection.ConfigureDocument(document, version);
                    options.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = localizer["Input your Bearer token in this format - Bearer {your token here} to access this API"],
                    })
                        .OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));


                    //options.AddSecurity("bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    //{
                    //    Type = OpenApiSecuritySchemeType.OAuth2,
                    //    Description = "My Authentication",
                    //    Flow = OpenApiOAuth2Flow.Implicit,
                    //    Flows = new OpenApiOAuthFlows()
                    //    {
                    //        Implicit = new OpenApiOAuthFlow()
                    //        {
                    //            Scopes = new Dictionary<string, string>
                    //            {
                    //                {"api1", "My API"}

                    //            },
                    //            TokenUrl = "http://localhost:5000/connect/token",
                    //            AuthorizationUrl = "http://localhost:5000/Login",

                    //        },
                    //    }
                    //});

                    //options.OperationProcessors.Add(
                    //    new AspNetCoreOperationSecurityScopeProcessor("bearer"));

                }

                services.AddSwaggerDocument(Configure).AddOpenApiDocument(document =>
                {
                    Configure(document);
                    document.DocumentName = "openapi/" + document.DocumentName;
                });
            }

            return services;
        }

        private static void ConfigureDocument(this IConfigurationSection configSection, OpenApiDocument document, ApiVersion version)
        {
            //document.Info.TermsOfService = "/terms/ofuse/url";
            
            configSection.GetSection("Contact").Bind(document.Info.Contact ?? (document.Info.Contact = new OpenApiContact()));
            configSection.GetSection("License").Bind(document.Info.License ?? (document.Info.License = new OpenApiLicense()));
            var prefix = "/api/" + ApiVersions.DocumentVersionPrefix + version.MajorVersion;
            foreach (var pair in document.Paths.ToArray())
            {
                if (pair.Key.Contains(prefix))
                {
                    document.Paths.Remove(pair.Key);
                    document.Paths[pair.Key.Substring(prefix.Length)] = pair.Value;
                }
            }
        }

        internal static IServiceCollection AddSerialization(this IServiceCollection services)
        {
            services
                .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    if (configureOptions.JsonSerializerOptions.Converters.All(c => c.GetType() != typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
            services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

            services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it
            return services;
        }

        internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.EnableSensitiveDataLogging(false);
                //options.UseLazyLoadingProxies();  // TODO: Problems with chat service at this moment
                //options.UseInMemoryDatabase("CoworkeeDb");

                var connectionStr = configuration?.GetConnectionString("postgresdb");
                if (!string.IsNullOrWhiteSpace(connectionStr))
                {
                    options.UseNpgsql(connectionStr);
                    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                }
                else
                    options.UseSqlServer(configuration?.GetConnectionString("Database") ?? configuration.GetConnectionString("DefaultConnection"));
            }).AddTransient<IDatabaseSeeder, DatabaseSeeder>();
            return services;
        }

        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        internal static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, ServerConfiguration config)
        {
            var key = Encoding.ASCII.GetBytes(config.AppConfiguration.Secret);

            async void ConfigureOptions(JwtBearerOptions bearer)
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };

                var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                bearer.Events = new JwtBearerEvents
                {
                    OnTokenValidated = c =>
                    {
                        var userId = c.Principal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                        if (!string.IsNullOrEmpty(userId)) c.HttpContext?.Session?.SetString(ApplicationConstants.Session.SessionUserIdKey, userId);
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception is SecurityTokenExpiredException)
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail(localizer["The Token is expired."]));
                            c.HttpContext?.RequestServices?.GetService<IAccountService>()?.LogoutAsync();
                            return c.Response.WriteAsync(result);
                        }
                        else
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                            return c.Response.WriteAsync(result);
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not Authorized."]));
                            return context.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not authorized to access this resource."]));
                        return context.Response.WriteAsync(result);
                    },
                };
            }

            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(ConfigureOptions);
            services.AddAuthorization(options =>
            {
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null)?.ToString();
                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue));
                    }
                }
            });
            return services;
        }
    }
}