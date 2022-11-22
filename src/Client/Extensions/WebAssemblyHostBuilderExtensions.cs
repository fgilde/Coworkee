using Blazored.LocalStorage;
using CleanArchitectureBase.Shared.Constants.Permission;
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
using CleanArchitectureBase.Client.Authentication;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Client.ErrorHandling;
using CleanArchitectureBase.Client.Handler;
using CleanArchitectureBase.Client.Localization;
using CleanArchitectureBase.Client.Managers;
using CleanArchitectureBase.Client.Managers.ExtendedAttribute;
using CleanArchitectureBase.Client.Managers.Preferences;
using CleanArchitectureBase.Domain.Entities.ExtendedAttributes;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.SDK;
using CleanArchitectureBase.Shared;
using CleanArchitectureBase.Shared.Constants.Application;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Client.Configuration.MudExObjectEdit;
using Toolbelt.Blazor.Extensions.DependencyInjection;
namespace CleanArchitectureBase.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
        {
            //builder.RootComponents.Add<App>("#app");
            builder.RootComponents.RegisterCustomElement<App>("blazor-app");
            return builder;
        }
        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
            var clientSettings = ClientApplicationConfiguration.Create(builder.Configuration);
            builder
            .Services
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
                .AddManagers()
                .AddExtendedAttributeManagers()
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ApplicationConstants.ApplicationClientName).EnableIntercept(sp))
                .AddHttpClient(ApplicationConstants.ApplicationClientName, client =>
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
            builder.Services.AddGrpcDataClient((services, options) =>
            {
                var authEnabledHandler = services.GetRequiredService<AuthenticationHeaderHandler>();
                //var client = services.GetRequiredService<HttpClient>();
                authEnabledHandler.InnerHandler = new HttpClientHandler();
                options.BaseUri = clientSettings.BackendOrigin;
                options.MessageHandler = authEnabledHandler;
            });

            builder.Services.AddHttpClientInterceptor();
           
            // builder.Services.AddSingleton<HubConnection>(sp => HubExtensions.BuildHubConnection(clientSettings.BackendOrigin));

            return builder;
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
                return new CleanArchitectureBase.Data.CleanArchitectureBaseData.CleanArchitectureBaseDataClient(channel);
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