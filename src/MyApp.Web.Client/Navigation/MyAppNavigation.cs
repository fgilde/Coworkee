using Coworkee.Client.Blazor;
using Coworkee.Client.Blazor.Navigation;
using MudBlazor;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;

namespace MyApp.Web.Client.Navigation;

internal sealed class MyAppNavigation : INavigationContributor
{
    private const string Personal = "Personal";
    private const string DocumentManagement = "Document Management";
    private const string Communication = "Communication";
    private const string CatalogManagement = "Catalog Management";

    public static void Order(NavigationMenuOptions menu) => menu
        .OrderGroup(Personal, 0)
        .OrderGroup(DocumentManagement, 1)
        .OrderGroup(NavigationGroups.Administration, 2)
        .OrderGroup(Communication, 3)
        .OrderGroup(CatalogManagement, 4)
        .Place("/admin/audit", Personal, "Audit Trails", 2)
        .Place("/admin/settings", NavigationGroups.System, "Site Settings", -1)
        .Place("/admin/backups", NavigationGroups.System, "Database Backups", 5)
        .Place("/chat", Communication)
        .IconForGroup(Communication, MudBlazor.Icons.Material.Outlined.Forum)
        .IconForGroup(Personal, MudBlazor.Icons.Material.Outlined.Person)
        .IconForGroup(DocumentManagement, MudBlazor.Icons.Material.Outlined.Description)
        .IconForGroup(CatalogManagement, MudBlazor.Icons.Material.Outlined.Inventory2);

    public IEnumerable<CoworkeeNavItem> Items =>
    [
        new("Dashboard", "/dashboard", Icons.Material.Outlined.Dashboard, CatalogPermissions.Dashboards.View, Group: Personal),
        new("Account", "/profile", Icons.Material.Outlined.ManageAccounts, Group: Personal, Order: 1),
        new("Document Store", "/document-store", Icons.Material.Outlined.AttachFile, DocumentPermissions.Documents.View, Group: DocumentManagement),
        new("Document Types", "/document-types", Icons.Material.Outlined.FileCopy, DocumentPermissions.Types.View, Group: DocumentManagement, Order: 1),
        new("Products", "/catalog/products", Icons.Material.Outlined.ViewCarousel, CatalogPermissions.Products.View, Group: CatalogManagement),
        new("Brands", "/catalog/brands", Icons.Material.Outlined.Sell, CatalogPermissions.Brands.View, Group: CatalogManagement, Order: 1),
    ];
}
