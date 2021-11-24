using System.Collections.Generic;
using CleanArchitectureBase.Shared.Constants.Permission;
using MudBlazor;

namespace CleanArchitectureBase.Client.Models.Navigation
{
    public static class Navigations
    {
        public static HashSet<NavigationEntry> Default(string backendOrigin) => new()
        {
            new NavigationEntry("Home", Icons.Material.Outlined.Home, "/"),
            new NavigationEntry("Hangfire", Icons.Material.Outlined.Work, $"{backendOrigin}/jobs", "_blank").WithPolicies(Permissions.Hangfire.View),
            new NavigationEntry("Swagger", Icons.Material.Outlined.LiveHelp, $"{backendOrigin}/swagger/index.html", "_blank").WithPolicies(Permissions.Swagger.View),
            new NavigationEntry("Personal")
            {
                Children = new()
                {
                    new NavigationEntry("Dashboard", Icons.Material.Outlined.Dashboard, "/dashboard").WithPolicies(Permissions.Dashboards.View),
                    new NavigationEntry("Account", Icons.Material.Outlined.SupervisorAccount, "/account").WithAuthentication(),
                    new NavigationEntry("Audit Trails", Icons.Material.Outlined.Security, "/audit-trails").WithPolicies(Permissions.AuditTrails.View),
                }
            },
            new NavigationEntry("Document Management")
            {
                Children = new()
                {
                    new NavigationEntry("Document Store", Icons.Material.Outlined.AttachFile, "/document-store").WithPolicies(Permissions.Documents.View),
                    new NavigationEntry("Document Types", Icons.Material.Outlined.AttachFile, "/document-types").WithPolicies(Permissions.DocumentTypes.View)
                }
            },
            new NavigationEntry("Administrator")
            {
                Children = new()
                {
                    new NavigationEntry("Users", Icons.Material.Outlined.Person, "/identity/users").WithPolicies(Permissions.Users.View),
                    new NavigationEntry("Roles", Icons.Material.Outlined.Person, "/identity/roles").WithPolicies(Permissions.Roles.View),
                    new NavigationEntry("Localization")
                    {
                        Children = new()
                        {
                            new NavigationEntry("Languages", Icons.Material.Outlined.Language, "/localization/languages").WithPolicies(Permissions.Translations.Edit),
                            new NavigationEntry("Translations", Icons.Material.Outlined.Translate, "/localization/translations").WithPolicies(Permissions.Translations.Edit)
                        }
                    }
                }
            },
            new NavigationEntry("Communication")
            {
                Children = new()
                {
                    new NavigationEntry("Chat", Icons.Material.Outlined.Chat, "/chat").WithPolicies(Permissions.Communication.Chat)
                }
            },
            new NavigationEntry("Catalog Management")
            {
                Children = new()
                {
                    new NavigationEntry("Products", Icons.Material.Outlined.CallToAction, "/catalog/products").WithPolicies(Permissions.Products.View),
                    new NavigationEntry("Brands", Icons.Material.Outlined.CallToAction, "/catalog/brands").WithPolicies(Permissions.Brands.View)
                }
            }
        };
    }
}