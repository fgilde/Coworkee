using Coworkee.Contracts.Chat;
using Coworkee.Contracts.Reporting;
using Coworkee.Contracts.Social;
using Coworkee.Contracts.Workflows;
using Coworkee.Identity.Setup;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;
using MyApp.Contracts.Sales;

namespace MyApp.Migrations;

/// <summary>Development seed with the users of <see cref="DemoUsers"/>; change their passwords before you deploy anywhere.</summary>
public static class DemoSeed
{
    public const string SalesManager = "Sales Manager";

    public static readonly DemoUser Administrator = DemoUsers.Administrator;

    public static void Configure(IdentitySeedOptions seed)
    {
        seed.TenantName = "MyApp";
        seed.Roles.Add(new SeedRole("Product Manager", "Maintains products", [.. Shared, CatalogPermissions.Products.View, CatalogPermissions.Products.Create, CatalogPermissions.Products.Edit, CatalogPermissions.Products.Delete, CatalogPermissions.Brands.View,
            SocialPermissions.Tags.View, SocialPermissions.Tags.Create, SocialPermissions.Comments.View, SocialPermissions.Comments.Create, SocialPermissions.Ratings.View, SocialPermissions.Ratings.Create, ReportPermissions.Manage]));
        seed.Roles.Add(new SeedRole(SalesManager, "Follows orders and revenue in reports", [.. Shared, SalesPermissions.Orders.View, ReportPermissions.Manage]));
        seed.Roles.Add(new SeedRole("Brand Manager", "Maintains brands", [.. Shared, CatalogPermissions.Brands.View, CatalogPermissions.Brands.Create, CatalogPermissions.Brands.Edit, CatalogPermissions.Brands.Delete]));
        seed.Roles.Add(new SeedRole("Customer", "Registers to see the dashboard and keep own documents",
            [CatalogPermissions.Dashboards.View, DocumentPermissions.Documents.View, DocumentPermissions.Documents.Create], SelectableForRegistration: true));
        seed.Users.AddRange(DemoUsers.All.Select(u => new SeedUser(u.Email, u.Password, u.FirstName, u.LastName, u.IsAdmin) { Roles = u.Roles ?? [] }));
    }

    /// <summary>A deployment: the organisation and the administrator of the deploy parameters, no demo users.</summary>
    public static void ConfigureDeployment(IdentitySeedOptions seed, SeedUser admin)
    {
        seed.TenantName = "MyApp";
        seed.Users.Add(admin);
    }

    private static string[] Shared =>
    [
        CatalogPermissions.Dashboards.View,
        ChatPermissions.Use,
        ReportPermissions.View,
        WorkflowPermissions.View, WorkflowPermissions.Transition,
        DocumentPermissions.Documents.View, DocumentPermissions.Documents.Create, DocumentPermissions.Documents.Edit, DocumentPermissions.Documents.Delete,
    ];
}
