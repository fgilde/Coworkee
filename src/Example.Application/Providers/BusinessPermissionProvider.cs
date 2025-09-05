using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coworkee.Core.Providers;
using lib.Coworkee.Shared.Constants.Permission;

namespace Example.Application.Providers
{
    /// <summary>
    /// Business-specific permission provider for the example application.
    /// Demonstrates how to extend core permissions with business features.
    /// </summary>
    public class BusinessPermissionProvider : PermissionProviderBase
    {
        /// <summary>
        /// Business-specific permissions for this application.
        /// </summary>
        public static class Business
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
            return typeof(Business).GetNestedTypes()
                .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Select(info => (info, info.GetValue(null)?.ToString())).Where(o => o.Item2 is not null);
        }
    }
}