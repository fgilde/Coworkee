using System;
using System.Linq;
using lib.Coworkee.Shared.Constants.Permission;
using lib.Coworkee.Shared.Constants.Role;
using lib.Coworkee.Shared.Models;

namespace Coworkee.Shared.Constants.Application
{
    /// <summary>
    /// Business-specific permissions for the application.
    /// Core permissions are provided by lib.Coworkee.Shared.Constants.Permission.CorePermissionProvider
    /// </summary>
    public static class BusinessPermissions
    {
        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Create = "Permissions.Products.Create";
            public const string Edit = "Permissions.Products.Edit";
            public const string Delete = "Permissions.Products.Delete";
            public const string Export = "Permissions.Products.Export";
            public const string Search = "Permissions.Products.Search";
        }

        public static class Brands
        {
            public const string View = "Permissions.Brands.View";
            public const string Create = "Permissions.Brands.Create";
            public const string Edit = "Permissions.Brands.Edit";
            public const string Delete = "Permissions.Brands.Delete";
            public const string Export = "Permissions.Brands.Export";
            public const string Search = "Permissions.Brands.Search";
        }

        public static class Dashboards
        {
            public const string View = "Permissions.Dashboards.View";
        }
    }

    public static class ApplicationConstants
    {
       // public const string LargeLanguageModel = "deepseek-r1:32b";
        public const string LargeLanguageModel = "deepseek-r1";
        public static bool IsNswagGeneration { get; set; }
        public const string ApplicationName = "Coworkee";
        public const string? DatabaseName = "CoworkeeDb";

        public static string AspireServerAppName = (HostClientInServer ? $"{ApplicationName}-Application" : $"{ApplicationName}-Server-Api").ToLower();
        public static string AspireClientAppName = ($"{ApplicationName}-Client-Application").ToLower();
        public const string KeycloakRealm = "master";
        public const string ApplicationClientName = $"{ApplicationName}Client";
        public const string SessionIdKey = nameof(SessionIdKey);
        public const string DefaultLanguageCode = "en-US";
        public const string DefaultDocumentTypeName = "Unassigned";
        public const string KeycloakSchemeName = "keycloak";


#if HostClient
        public const bool HostClientInServer = true;
#else
        public const bool HostClientInServer = false;
#endif

        public static class ServiceNames
        {
            public const string Ollama = nameof(Ollama);
            public const string OllamaUI = nameof(OllamaUI);
            public const string Keycloak = KeycloakSchemeName;
            public const string Grafana = nameof(Grafana);
            public const string Prometheus = nameof(Prometheus);
            public const string PgAdmin = nameof(PgAdmin);
            public const string Stirling = nameof(Stirling);
            public const string KeycloakAdmin = nameof(KeycloakAdmin);
            public const string Postgress = nameof(Postgress);
            public const string SqlServer = nameof(SqlServer);
            public const string Storage = "storage";
            public const string Blobs = "blobs";
        }

        public static class Defaults
        {
            public const string ApplicationClientSecret = $"{ApplicationClientName}-34C2F2F8-CD3D-4976-AA08-36AE10FA4119";

            public static class Users
            {
                public static CreateUser System => new(nameof(System), nameof(System), ApplicationName, $"{nameof(System)}@{ApplicationName}", "SystemUserPassw0rd4SystemUserAccess73F1985F3C1A4158B4BA70F7B65778BF", true, CoreRoleConstants.AdministratorRole);
                public static CreateUser[] Administrators => new[]
                {
                    new CreateUser("admin", "Administrator", "", "info@coworkee.de","123Pa$$word!", true, CoreRoleConstants.AdministratorRole),
                    new CreateUser("fgilde", "Florian", "Gilde", "fgilde@gmail.com","123Pa$$word!", true, CoreRoleConstants.AdministratorRole)
                };
                public static CreateUser[] Basic => new[]
                {
                    new CreateUser("johndoe", "John", "Doe", "john@coworkee.de","123Pa$$word!", false, CoreRoleConstants.BasicRole)
                };
            }

            public static (string Name, bool SelectableOnRegistration, string[] Permissions)[] Roles = 
            {
                ("Product Manager", true, new []
                {
                    BusinessPermissions.Dashboards.View,
                    CorePermissionProvider.Core.Communication.Chat,
                    CorePermissionProvider.Core.Documents.View,
                    CorePermissionProvider.Core.Documents.Create,
                    CorePermissionProvider.Core.Documents.Edit,
                    CorePermissionProvider.Core.Documents.Delete,
                    BusinessPermissions.Products.Create,
                    BusinessPermissions.Products.Edit,
                    BusinessPermissions.Products.Delete,
                    BusinessPermissions.Products.View,
                    BusinessPermissions.Brands.View               
                }),
                ("Brand Manager", true, new []
                {
                    BusinessPermissions.Dashboards.View,
                    CorePermissionProvider.Core.Communication.Chat,
                    CorePermissionProvider.Core.Documents.View,
                    CorePermissionProvider.Core.Documents.Create,
                    CorePermissionProvider.Core.Documents.Edit,
                    CorePermissionProvider.Core.Documents.Delete,
                    BusinessPermissions.Brands.Create,
                    BusinessPermissions.Brands.Edit,
                    BusinessPermissions.Brands.Delete,
                    BusinessPermissions.Brands.View
                })
            };
        }

        public static class Routes
        {
            public const string Login = nameof(Login);
            public const string Redirect = nameof(Redirect);
            public const string Register = nameof(Register);
            public const string Forbidden = nameof(Forbidden);
            public const string Dashboard = "/jobs";
            public const string ApiDocumentation = "/swagger/index.html";

            public static string[] AuthRequired = [Dashboard, ApiDocumentation];
            public static bool IsAuthRequired(string route) => AuthRequired.Contains(route, StringComparer.InvariantCultureIgnoreCase);
        }

        public static class FileAccess
        {
            public const string StaticFileDirectoryName = "Files";
            public const string OverridingSettingsFile = "appsettings.Override.json"; // In this file admin settings are stored. This file then overrides the appsettings.json file.
        }

        public static class ParameterNames
        {
            public const string ReturnUrl = nameof(ReturnUrl);
            public const string AuthedUrlParameter = "auth_token";
            public const string IdToken = "id_token";
            public static string Build(string key, params string[] values)
            {
                return values.Where(s => !string.IsNullOrWhiteSpace(s)).Aggregate(key, (current, value) => current + ("_" + value));
            }
        }

        public static class HeaderNames
        {
            public const string RoleIdHeader = "x-role-id";
        }

        public static class Environment
        {
            public const string Testing = nameof(Testing);
            public const string Development = nameof(Development);
            public const string Production = nameof(Production);
        }

        public static class Session
        {
            public const int RefreshTokenExpiryInDays = 7;
            public const int SecurityTokenExpiryInDays = 2;
            public const string SessionUserIdKey = nameof(Session) + "_" + nameof(SessionUserIdKey);
        }

        public static class ServiceBusQueues
        {
            public const string TestQueue = nameof(TestQueue);
            public const string ClientEventQueue = nameof(ClientEventQueue);
            public const string MainQueue = nameof(MainQueue);
        }

        public static class SignalR
        {
            public const string Resource = "signalr";
            public const string EventHubUrl = "/eventHub";
            public const string ClientEventName = "EventRecieved";
        }
        public static class Cache
        {
            public static string CacheKeyFor(Type type, params string[] keys)
            {
                return keys.Where(s => !string.IsNullOrWhiteSpace(s)).Aggregate($"{CacheKey}-{type.FullName}", (current, key) => current + ("-" + key));
            }

            public const string CacheKey = "all-of";

            public static string GetAllEntityExtendedAttributesCacheKey(string entityFullName)
            {
                return $"all-{entityFullName}-extended-attributes";
            }

            public static string GetAllEntityExtendedAttributesByEntityIdCacheKey<TEntityId>(string entityFullName, TEntityId entityId)
            {
                return $"all-{entityFullName}-extended-attributes-{entityId}";
            }
        }
    }
}