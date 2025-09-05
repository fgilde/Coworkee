using Blazored.LocalStorage;
using lib.Coworkee.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Coworkee.Client.Authentication;
using Coworkee.Client.Configuration;
using Coworkee.Client.ErrorHandling;
using Coworkee.Client.Handler;
using Coworkee.Client.Localization;
using Coworkee.Client.Managers;
using Coworkee.Client.Managers.ExtendedAttribute;
using Coworkee.Client.Managers.Preferences;
using lib.Coworkee.Domain.Entities.ExtendedAttributes;
using lib.Coworkee.Domain.Entities.Misc;
using Coworkee.SDK;
using Coworkee.Shared;
using lib.Coworkee.Shared.Constants.Application;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Nextended.Core.Extensions;
using lib.Coworkee.Application.Contracts.Services;
using Coworkee.Client.Configuration.MudExObjectEdit;
using Nextended.Core.Helper;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Coworkee.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {

        internal static WebAssemblyHostBuilder DiscoverBackendUrlIfNeeded(this WebAssemblyHostBuilder builder)
        {
            var dockerBackendOriginValue = Environment.GetEnvironmentVariable("BACKEND_ORIGIN") ?? builder.Configuration["DockerBackendOrigin"];
            if (!string.IsNullOrEmpty(dockerBackendOriginValue) && dockerBackendOriginValue != "__BACKEND_ORIGIN__")
            {
                builder.Configuration[nameof(ClientApplicationConfiguration.BackendOrigin)] = dockerBackendOriginValue;
                return builder;
            }
            if (string.IsNullOrEmpty(builder.Configuration[nameof(ClientApplicationConfiguration.BackendOrigin)]))
            {
                if (ApplicationConstants.HostClientInServer) {
                    builder.Configuration[nameof(ClientApplicationConfiguration.BackendOrigin)] = builder.HostEnvironment.BaseAddress.EnsureEndsWith("/")[..^1];
                }
                else
                {
                    var app = builder.Configuration.GetSection("Services")?.GetSection(ApplicationConstants.AspireServerAppName);
                    var serviceUrl = (app?.GetSection("https") ?? app.GetSection("http"))?.GetSection("0")?.Value;
                    builder.Configuration[nameof(ClientApplicationConfiguration.BackendOrigin)] = serviceUrl;
                }
            }
            return builder;
        }

        public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#app");
            //builder.RootComponents.RegisterCustomElement<App>("blazor-app");
            return builder;
        }
        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {   
            var clientSettings = ClientApplicationConfiguration.Create(builder.Configuration);
            var logLevel = clientSettings.Logging.LogLevel.Default.ToEnum<LogLevel>();
            builder.Logging.SetMinimumLevel(logLevel);
            Console.WriteLine($"Backend Origin: {clientSettings.BackendOrigin}");
            builder.Services.AddClientServices(clientSettings);
            return builder;
        }


        public static IServiceCollection AddClientServices(this IServiceCollection services, ClientApplicationConfiguration clientSettings)
        {
            services
                .AddTransient(p => clientSettings)
                .AddTransient(p => p.GetService<ClientApplicationConfiguration>()?.ServerConfiguration)
                .AddLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                })
                .AddTransient(typeof(IStringLocalizer<>), typeof(ApiLocalizer<>))
                .AddYamlLocalizationWithFallback()
                .AddAuthorizationCore(RegisterPermissionClaims)
                .AddBlazoredLocalStorage()
                .AddMudServices(configuration =>
                {
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = false;
                })
                .AddMudExWithExtendedDefaults()
                .AddScoped<ClientPreferenceManager>()
                .AddScoped<IClientPreferenceManager, ClientPreferenceManager>()
                .AddScoped<ApplicationStateProvider>()
                .AddScoped<AuthenticationStateProvider, ApplicationStateProvider>()
                .AddScoped<ICurrentUserService, ApplicationStateProvider>(p => p.GetService<ApplicationStateProvider>())
                .AddTransient<IErrorHandler, ErrorHandler>()
                .AddScoped<IHealthChecker, HealthChecker>()
                .AddManagers()
                .AddExtendedAttributeManagers()
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ApplicationConstants.ApplicationClientName).EnableIntercept(sp))
                .AddHttpClient(ApplicationConstants.ApplicationClientName, (provider, client) =>
                {
                    client.UpdateAcceptLanguage();
                    client.BaseAddress = new Uri(clientSettings.BackendOrigin);
                })
                .AddTypedClient<IApplicationClient>((_, services) =>
                {
                    var c = services.GetService<HttpClient>();
                    return new ApplicationClient(c.BaseAddress.AbsoluteUri.EnsureEndsWith("/") + "api/v1/", c);
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            // .AddHttpMessageHandler<AuthorizationMessageHandler>() // TODO: IDENTITY SERVER Not sure


            // gRPC-Web client with auth
            services.AddGrpcDataClient((services, options) =>
            {
                var authEnabledHandler = services.GetRequiredService<AuthenticationHeaderHandler>();
                var client = services.GetRequiredService<HttpClient>();
                authEnabledHandler.InnerHandler = new HttpClientHandler();
                //options.BaseUri = clientSettings.BackendOrigin;
                options.BaseUri = client.BaseAddress.AbsoluteUri;
                options.MessageHandler = authEnabledHandler;
            });

            services.AddHttpClientInterceptor();

            // builder.Services.AddSingleton<HubConnection>(sp => HubExtensions.BuildHubConnection(clientSettings.BackendOrigin));
            return services;
        }

        private static void AddGrpcDataClient(this IServiceCollection serviceCollection, Action<IServiceProvider, MainGrpcDataClientOptions> configure)
        {
            serviceCollection.AddScoped(services =>
            {
                var options = new MainGrpcDataClientOptions();
                configure(services, options);
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, options.MessageHandler!));

                return GrpcChannel.ForAddress(options.BaseUri!, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null });
            });
            serviceCollection.AddScoped(services =>
            {
                var channel = services.GetRequiredService<GrpcChannel>();
                return new Coworkee.Data.CoworkeeData.CoworkeeDataClient(channel);
            });
        }

        private static IServiceCollection AddManagers(this IServiceCollection services)
        {
            var managers = typeof(IManager);

            var types = managers
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (managers.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }

        private static IServiceCollection AddExtendedAttributeManagers(this IServiceCollection services)
        {
            //TODO - add managers with reflection!

            return services
                .AddTransient(typeof(IExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>), typeof(ExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>));
        }

        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                }
            }
        }
    }

    public class MainGrpcDataClientOptions
    {
        public string? BaseUri { get; set; }
        public HttpMessageHandler? MessageHandler { get; set; }
    }
}