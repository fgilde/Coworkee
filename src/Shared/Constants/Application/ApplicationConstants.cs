using System;
using System.Linq;
using CleanArchitectureBase.Shared.Constants.Role;
using CleanArchitectureBase.Shared.Models;

namespace CleanArchitectureBase.Shared.Constants.Application
{
    public static class ApplicationConstants
    {
        public const string ApplicationName = "CleanArchitectureBase";
        public static string ApplicationClientName = $"{ApplicationName}Client";
        public const string SessionIdKey = nameof(SessionIdKey);
        public const string Version = "v2.2";
        public const string DefaultLanguageCode = "en-US";
        
        public static class Defaults
        {
            public static class Users
            {
                public static CreateUser System => new(nameof(System), nameof(System), ApplicationName, $"{nameof(System)}@{ApplicationName}", "SystemUserPassw0rd4SystemUserAccess73F1985F3C1A4158B4BA70F7B65778BF", true, RoleConstants.AdministratorRole);
                public static CreateUser[] Administrators => new[]
                {
                    new CreateUser("fgilde", "Florian", "Gilde", "info@coworkee.de","123Pa$$word!", true, RoleConstants.AdministratorRole) 
                };
                public static CreateUser[] Basic => new[]
                {
                    new CreateUser("johndoe", "John", "Doe", "john@coworkee.de","123Pa$$word!", false, RoleConstants.BasicRole)
                };
            }
        }

        public static class Routes
        {
            public const string Login = nameof(Login);
            public const string Register = nameof(Register);
            public const string Forbidden = nameof(Forbidden);
        }

        public static class ParameterNames
        {
            public const string ReturnUrl = nameof(ReturnUrl);
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
            public const string SessionUserIdKey = nameof(Session) + "_" + nameof(SessionUserIdKey);
        }

        public static class Hangfire
        {
            public const string DashboardRoute = "/jobs";
        }

        public static class ServiceBusQueues
        {
            public const string TestQueue = nameof(TestQueue);
            public const string ClientEventQueue = nameof(ClientEventQueue);
            public const string MainQueue = nameof(MainQueue);
        }

        public static class SignalR
        {
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

        public static class MimeTypes
        {
            public const string Csv = "text/csv";
            public const string OpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            public const string Xls = "application/vnd.ms-excel";
        }
    }
}