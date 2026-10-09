using Coworkee.Contracts.Chat;
using Coworkee.Identity.Setup;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;

namespace MyApp.Migrations;

public static class DemoSeed
{
    public static readonly SeedUser Administrator = new SeedUser("info@coworkee.de", "123Pa$$word!", "Administrator", null, IsAdmin: true);

    public static readonly SeedUser Florian = new SeedUser("fgilde@gmail.com", "123Pa$$word!", "Florian", "Gilde", IsAdmin: true);

    public static readonly SeedUser John = new SeedUser("john@coworkee.de", "123Pa$$word!", "John", "Doe");

    public static void Configure(IdentitySeedOptions seed)
    {
        seed.TenantName = "MyApp";
        seed.Roles.Add(new SeedRole("Product Manager", "Maintains products", [.. Shared, CatalogPermissions.Products.View, CatalogPermissions.Products.Create, CatalogPermissions.Products.Edit, CatalogPermissions.Products.Delete, CatalogPermissions.Brands.View]));
        seed.Roles.Add(new SeedRole("Brand Manager", "Maintains brands", [.. Shared, CatalogPermissions.Brands.View, CatalogPermissions.Brands.Create, CatalogPermissions.Brands.Edit, CatalogPermissions.Brands.Delete]));
        seed.Roles.Add(new SeedRole("Customer", "Registers to see the dashboard and keep own documents",
            [CatalogPermissions.Dashboards.View, DocumentPermissions.Documents.View, DocumentPermissions.Documents.Create], SelectableForRegistration: true));
        seed.Users.AddRange([Administrator, Florian, John]);
    }

    private static string[] Shared =>
    [
        CatalogPermissions.Dashboards.View,
        ChatPermissions.Use,
        DocumentPermissions.Documents.View, DocumentPermissions.Documents.Create, DocumentPermissions.Documents.Edit, DocumentPermissions.Documents.Delete,
    ];
}
