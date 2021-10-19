using System.Collections.Generic;
using CleanArchitectureBase.Shared.Constants.Permission;
using MudBlazor;

namespace CleanArchitectureBase.Client.Models.Navigation
{
    public static class Navigations
    {
        public static HashSet<NavigationEntry> Default = new()
        {
            new NavigationEntry("Home",Icons.Material.Outlined.Home, "/" ),
            new NavigationEntry("Hangfire",Icons.Material.Outlined.Work, "/jobs", "_blank" ).WithPolicies(Permissions.Hangfire.View),
            new NavigationEntry("Swagger",Icons.Material.Outlined.LiveHelp, "/swagger/index.html", "_blank" ).WithPolicies(Permissions.Swagger.View),
            new NavigationEntry("Personal")
            {
                Entries = new()
                {
                    new NavigationEntry("Dashboard",Icons.Material.Outlined.Dashboard, "/dashboard" ),
                    new NavigationEntry("Account",Icons.Material.Outlined.SupervisorAccount, "/account" ),
                    new NavigationEntry("Audit Trails",Icons.Material.Outlined.Security, "/audit-trails" ).WithPolicies(Permissions.AuditTrails.View),
                }
            },
            new NavigationEntry("Document Management")
            {
                Entries = new()
                {
                    new NavigationEntry("Document Store",Icons.Material.Outlined.AttachFile, "/document-store").WithPolicies(Permissions.Documents.View),
                    new NavigationEntry("Document Types",Icons.Material.Outlined.AttachFile, "/document-types").WithPolicies(Permissions.DocumentTypes.View)
                }
            },
            new NavigationEntry("Administrator")
            {
                Entries = new()
                {
                    new NavigationEntry("Users",Icons.Material.Outlined.Person, "/identity/users").WithPolicies(Permissions.Users.View),
                    new NavigationEntry("Roles",Icons.Material.Outlined.Person, "/identity/roles").WithPolicies(Permissions.Roles.View)
                }
            },
            new NavigationEntry("Communication")
            {
                Entries = new()
                {
                    new NavigationEntry("Chat",Icons.Material.Outlined.Chat, "/chat").WithPolicies(Permissions.Communication.Chat)
                }
            },
            new NavigationEntry("Catalog Management")
            {
                Entries = new()
                {
                    new NavigationEntry("Products",Icons.Material.Outlined.CallToAction, "/catalog/products").WithPolicies(Permissions.Products.View),
                    new NavigationEntry("Brands",Icons.Material.Outlined.CallToAction, "/catalog/brands").WithPolicies(Permissions.Brands.View)
                }
            }
        };
    }
}