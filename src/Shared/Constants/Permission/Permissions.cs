using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CleanArchitectureBase.Shared.Constants.Permission
{
    public static class Permissions
    {
        public static class Products
        {
            public const string View = "Permissions.Products.View";
            [RequiresPermissions(Brands.View, View)] // You can't create products without to see possible brands for assignment
            public const string Create = "Permissions.Products.Create";
            [RequiresPermissions(Brands.View, View)] // You can't edit products without to see possible brands for assignment
            public const string Edit = "Permissions.Products.Edit";
            [RequiresPermissions(View)] 
            public const string Delete = "Permissions.Products.Delete";
            [RequiresPermissions(View)]
            public const string Export = "Permissions.Products.Export";
            [RequiresPermissions(View)]
            public const string Search = "Permissions.Products.Search";
        }

        public static class Brands
        {
            public const string View = "Permissions.Brands.View";
            [RequiresPermissions(View)]
            public const string Create = "Permissions.Brands.Create";
            [RequiresPermissions(View)]
            public const string Edit = "Permissions.Brands.Edit";
            [RequiresPermissions(View)]
            public const string Delete = "Permissions.Brands.Delete";
            [RequiresPermissions(View)]
            public const string Export = "Permissions.Brands.Export";
            [RequiresPermissions(View)]
            public const string Search = "Permissions.Brands.Search";
        }

        public static class Documents
        {
            public const string View = "Permissions.Documents.View";
            [RequiresPermissions(View)]
            public const string Create = "Permissions.Documents.Create";
            [RequiresPermissions(View)]
            public const string Edit = "Permissions.Documents.Edit";
            [RequiresPermissions(View)]
            public const string Delete = "Permissions.Documents.Delete";
            [RequiresPermissions(View)]
            public const string Search = "Permissions.Documents.Search";
        }

        public static class DocumentTypes
        {
            public const string View = "Permissions.DocumentTypes.View";
            [RequiresPermissions(View)]
            public const string Create = "Permissions.DocumentTypes.Create";
            [RequiresPermissions(View)]
            public const string Edit = "Permissions.DocumentTypes.Edit";
            [RequiresPermissions(View)]
            public const string Delete = "Permissions.DocumentTypes.Delete";
            [RequiresPermissions(View)]
            public const string Export = "Permissions.DocumentTypes.Export";
            [RequiresPermissions(View)]
            public const string Search = "Permissions.DocumentTypes.Search";
        }

        public static class Translations
        {
            public const string View = "Permissions.Translations.View";
            [RequiresPermissions(View)]
            public const string Create = "Permissions.Translations.Create";
            [RequiresPermissions(View)]
            public const string Edit = "Permissions.Translations.Edit";
            [RequiresPermissions(View)]
            public const string Delete = "Permissions.Translations.Delete";
            [RequiresPermissions(View)]
            public const string Export = "Permissions.Translations.Export";
            [RequiresPermissions(View)]
            public const string Search = "Permissions.Translations.Search";
        }

        public static class DocumentExtendedAttributes
        {
            public const string View = "Permissions.DocumentExtendedAttributes.View";
            [RequiresPermissions(View)]
            public const string Create = "Permissions.DocumentExtendedAttributes.Create";
            [RequiresPermissions(View)]
            public const string Edit = "Permissions.DocumentExtendedAttributes.Edit";
            [RequiresPermissions(View)]
            public const string Delete = "Permissions.DocumentExtendedAttributes.Delete";
            [RequiresPermissions(View)]
            public const string Export = "Permissions.DocumentExtendedAttributes.Export";
            [RequiresPermissions(View)]
            public const string Search = "Permissions.DocumentExtendedAttributes.Search";
        }

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

            //TODO - add permissions
        }

        public static class Dashboards
        {
            public const string View = "Permissions.Dashboards.View";
        }

        public static class Hangfire
        {
            public const string View = "Permissions.Hangfire.View";
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

        public static string[] GetRequiredDependencyPermissionsFor(string permission)
        {
            return (IteratePermissionProperties().FirstOrDefault(tuple => tuple.Value == permission).Info
                        ?.GetCustomAttributes<RequiresPermissionsAttribute>()?.SelectMany(a => a.Permissions) ??
                    Enumerable.Empty<string>()).ToArray();
        }

        /// <summary>
        /// Returns a list of Permissions.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetRegisteredPermissions()
        {
            return IteratePermissionProperties().Select(prop => prop.Value);
        }

        private static IEnumerable<(FieldInfo Info, string Value)> IteratePermissionProperties()
        {
            return typeof(Permissions).GetNestedTypes()
                .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Select(info => (info, info.GetValue(null)?.ToString())).Where(o => o.Item2 is not null);
        }
    }
}