using System;
using System.Linq;

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
            public const string DefaultAdminUserEmail = "info@coworkee.de";
            public const string DefaultAdminUserPassword = "123Pa$$word!";
            public const string DefaultBasicUserEmail = "john@coworkee.de";
            public const string DefaultBasicUserPassword = "123Pa$$word!";
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