using lib.Coworkee.Shared.Constants.Permission;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using lib.Coworkee.Core.Providers;

namespace lib.Coworkee.Shared.Constants.Permission
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
                public const string View = "CorePermissionProvider.Core.Users.View";
                [RequiresPermissions(View)]
                public const string Create = "CorePermissionProvider.Core.Users.Create";
                [RequiresPermissions(View)]
                public const string Edit = "CorePermissionProvider.Core.Users.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "CorePermissionProvider.Core.Users.Delete";
                [RequiresPermissions(View)]
                public const string Export = "CorePermissionProvider.Core.Users.Export";
                [RequiresPermissions(View)]
                public const string Search = "CorePermissionProvider.Core.Users.Search";
            }

            public static class Roles
            {
                public const string View = "CorePermissionProvider.Core.Roles.View";
                [RequiresPermissions(View)]
                public const string Create = "CorePermissionProvider.Core.Roles.Create";
                [RequiresPermissions(View)]
                public const string Edit = "CorePermissionProvider.Core.Roles.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "CorePermissionProvider.Core.Roles.Delete";
                [RequiresPermissions(View)]
                public const string Search = "CorePermissionProvider.Core.Roles.Search";
            }

            public static class RoleClaims
            {
                public const string View = "CorePermissionProvider.Core.RoleClaims.View";
                [RequiresPermissions(View)]
                public const string Create = "CorePermissionProvider.Core.RoleClaims.Create";
                [RequiresPermissions(View)]
                public const string Edit = "CorePermissionProvider.Core.RoleClaims.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "CorePermissionProvider.Core.RoleClaims.Delete";
                [RequiresPermissions(View)]
                public const string Search = "CorePermissionProvider.Core.RoleClaims.Search";
            }

            public static class Documents
            {
                public const string View = "CorePermissionProvider.Core.Documents.View";
                [RequiresPermissions(View, DocumentTypes.View)]
                public const string Create = "CorePermissionProvider.Core.Documents.Create";
                [RequiresPermissions(View, DocumentTypes.View)]
                public const string Edit = "CorePermissionProvider.Core.Documents.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "CorePermissionProvider.Core.Documents.Delete";
                [RequiresPermissions(View)]
                public const string Search = "CorePermissionProvider.Core.Documents.Search";
            }

            public static class DocumentTypes
            {
                public const string View = "CorePermissionProvider.Core.DocumentTypes.View";
                [RequiresPermissions(View)]
                public const string Create = "CorePermissionProvider.Core.DocumentTypes.Create";
                [RequiresPermissions(View)]
                public const string Edit = "CorePermissionProvider.Core.DocumentTypes.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "CorePermissionProvider.Core.DocumentTypes.Delete";
                [RequiresPermissions(View)]
                public const string Export = "CorePermissionProvider.Core.DocumentTypes.Export";
                [RequiresPermissions(View)]
                public const string Search = "CorePermissionProvider.Core.DocumentTypes.Search";
            }

            public static class DocumentExtendedAttributes
            {
                public const string View = "CorePermissionProvider.Core.DocumentExtendedAttributes.View";
                [RequiresPermissions(View)]
                public const string Create = "CorePermissionProvider.Core.DocumentExtendedAttributes.Create";
                [RequiresPermissions(View)]
                public const string Edit = "CorePermissionProvider.Core.DocumentExtendedAttributes.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "CorePermissionProvider.Core.DocumentExtendedAttributes.Delete";
                [RequiresPermissions(View)]
                public const string Export = "CorePermissionProvider.Core.DocumentExtendedAttributes.Export";
                [RequiresPermissions(View)]
                public const string Search = "CorePermissionProvider.Core.DocumentExtendedAttributes.Search";
            }

            public static class Backups
            {
                public const string View = "CorePermissionProvider.Core.Backups.View";
                [RequiresPermissions(View)]
                public const string Create = "CorePermissionProvider.Core.Backups.Create";
                [RequiresPermissions(View)]
                public const string Edit = "CorePermissionProvider.Core.Backups.Edit";
                [RequiresPermissions(View)]
                public const string Delete = "CorePermissionProvider.Core.Backups.Delete";
                [RequiresPermissions(View)]
                public const string Restore = "CorePermissionProvider.Core.Backups.Restore";
                [RequiresPermissions(View)]
                public const string Search = "CorePermissionProvider.Core.Backups.Search";
            }

            public static class Communication
            {
                public const string Chat = "CorePermissionProvider.Core.Communication.Chat";
            }

            public static class Preferences
            {
                public const string ChangeLanguage = "Permissions.Preferences.ChangeLanguage";
            }

            // System administration permissions
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
                public const string View = "CorePermissionProvider.Core.AuditTrails.View";
                [RequiresPermissions(View)]
                public const string Export = "CorePermissionProvider.Core.AuditTrails.Export";
                [RequiresPermissions(View)]
                public const string Search = "CorePermissionProvider.Core.AuditTrails.Search";
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