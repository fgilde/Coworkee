using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Infrastructure;
using CleanArchitectureBase.Infrastructure.Contexts;
using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Server.Localization;
using CleanArchitectureBase.Server.Managers.Preferences;
using CleanArchitectureBase.Server.Permission;
using CleanArchitectureBase.Server.Services;
using CleanArchitectureBase.Server.Settings;
using CleanArchitectureBase.Shared.Constants.Localization;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
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
using CleanArchitectureBase.Application.Contracts;
using CleanArchitectureBase.Application.Contracts.Serialization.Options;
using CleanArchitectureBase.Application.Contracts.Serialization.Serializers;
using CleanArchitectureBase.Application.Contracts.Serialization.Settings;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.Account;
using CleanArchitectureBase.Application.Serialization.JsonConverters;
using CleanArchitectureBase.Application.Serialization.Options;
using CleanArchitectureBase.Application.Serialization.Serializers;
using CleanArchitectureBase.Application.Serialization.Settings;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace CleanArchitectureBase.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {

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
            var localizer = serviceProvider.GetService<IStringLocalizer<T>>();
            await serviceProvider.DisposeAsync();
            return localizer;
        }

        private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
        {
            var storageService = serviceProvider.GetService<ServerPreferenceManager>();
            if (storageService != null)
            {
                // TODO - should implement ServerStorageProvider to work correctly!
                CultureInfo culture;
                var preference = await storageService.GetPreference() as ServerPreference;
                if (preference != null)
                    culture = new CultureInfo(preference.LanguageCode);
                else
                    culture = new CultureInfo(LocalizationConstants.DefaultLanguageCode);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
        {
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
            return services;
        }

        internal static ServerConfiguration AddApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient(p => configuration.BindTo<ServerConfiguration>()); // Important as func to have always updated settings
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
                    options.AllowReferencesWithProperties = true;
                    options.PostProcess = document => configSection.ConfigureDocument(document, version);
                    options.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = localizer["Input your Bearer token in this format - Bearer {your token here} to access this API"],
                    })
                        .OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                }

                services.AddSwaggerDocument(document => {
                            Configure(document);
                            // document.DocumentName = "swagger/" + document.DocumentName;
                        }).AddOpenApiDocument(document =>
                        {
                            Configure(document);
                            document.DocumentName = "openapi/" + document.DocumentName;
                        });
            }

            return services;
        }

        private static void ConfigureDocument(this IConfigurationSection configSection, OpenApiDocument document, ApiVersion version)
        {
            configSection.GetSection("Contact").Bind(document.Info.Contact ?? (document.Info.Contact = new OpenApiContact()));
            configSection.GetSection("License").Bind(document.Info.License ?? (document.Info.License = new OpenApiLicense()));
            var prefix = $"/api/" + ApiVersions.DocumentVersionPrefix + version.MajorVersion;
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

        internal static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
            => services
                .AddDbContext<ApplicationDbContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<IDatabaseSeeder, DatabaseSeeder>();


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
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(async bearer =>
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
                            var userId = c.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                            if (!string.IsNullOrEmpty(userId))
                                c.HttpContext?.Session?.SetString(ApplicationConstants.Session.SessionUserIdKey, userId);
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
                });
            services.AddAuthorization(options =>
            {
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                    }
                }
            });
            return services;
        }
    }
}