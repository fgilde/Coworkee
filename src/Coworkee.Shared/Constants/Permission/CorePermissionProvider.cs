using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coworkee.Core.Providers;

namespace Coworkee.Shared.Constants.Permission
{
    /// <summary>
    /// Core permission provider for base system permissions.
    /// Business-specific permissions should be provided by application-level permission providers.
    /// </summary>
    public class CorePermissionProvider : PermissionProviderBase
    {
        /// <summary>
        /// Core system permissions that are common across all applications.
        /// </summary>
        public static class Core
        {
            public static class Users
            {
                public const string View = "Permissions.Users.View";
                [RequiresPermissions(View)]
                public const string Create = "Permissions.Users.Create";
                [RequiresPermissions(View)]
                public const string Edit = "Permissions.Users.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "Permissions.Users.Delete";
                [RequiresPermissions(View)]
                public const string Export = "Permissions.Users.Export";
                [RequiresPermissions(View)]
                public const string Search = "Permissions.Users.Search";
            }

            public static class Roles
            {
                public const string View = "Permissions.Roles.View";
                [RequiresPermissions(View)]
                public const string Create = "Permissions.Roles.Create";
                [RequiresPermissions(View)]
                public const string Edit = "Permissions.Roles.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "Permissions.Roles.Delete";
                [RequiresPermissions(View)]
                public const string Search = "Permissions.Roles.Search";
            }

            public static class RoleClaims
            {
                public const string View = "Permissions.RoleClaims.View";
                [RequiresPermissions(View)]
                public const string Create = "Permissions.RoleClaims.Create";
                [RequiresPermissions(View)]
                public const string Edit = "Permissions.RoleClaims.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "Permissions.RoleClaims.Delete";
                [RequiresPermissions(View)]
                public const string Search = "Permissions.RoleClaims.Search";
            }

            public static class Communication
            {
                public const string Chat = "Permissions.Communication.Chat";
            }

            public static class Preferences
            {
                public const string ChangeLanguage = "Permissions.Preferences.ChangeLanguage";
            }

            public static class Dashboards
            {
                public const string View = "Permissions.Dashboards.View";
            }

            public static class Hangfire
            {
                public const string View = "Permissions.Hangfire.View";
            }

            public static class PgAdmin
            {
                public const string View = "Permissions.PgAdmin.View";
            }

            public static class Grafana
            {
                public const string View = "Permissions.Grafana.View";
            }

            public static class OllamaUI
            {
                public const string View = "Permissions.OllamaUI.View";
            }

            public static class Prometheus
            {
                public const string View = "Permissions.Prometheus.View";
            }

            public static class Swagger
            {
                public const string View = "Permissions.Swagger.View";
            }

            public static class AuditTrails
            {
                public const string View = "Permissions.AuditTrails.View";
                [RequiresPermissions(View)]
                public const string Export = "Permissions.AuditTrails.Export";
                [RequiresPermissions(View)]
                public const string Search = "Permissions.AuditTrails.Search";
            }
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetPermissions()
        {
            return IteratePermissionProperties().Select(prop => prop.Value);
        }

        /// <inheritdoc />
        public override string[] GetRequiredDependencyPermissions(string permission)
        {
            return (IteratePermissionProperties().FirstOrDefault(tuple => tuple.Value == permission).Info
                        ?.GetCustomAttributes<RequiresPermissionsAttribute>()?.SelectMany(a => a.Permissions) ??
                    Enumerable.Empty<string>()).ToArray();
        }

        private static IEnumerable<(FieldInfo Info, string Value)> IteratePermissionProperties()
        {
            return typeof(Core).GetNestedTypes()
                .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Select(info => (info, info.GetValue(null)?.ToString())).Where(o => o.Item2 is not null);
        }
    }
}