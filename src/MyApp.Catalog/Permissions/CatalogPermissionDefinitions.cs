using Coworkee.Application.Authorization;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Permissions;

internal sealed class CatalogPermissionDefinitions : IPermissionDefinitionContributor
{
    public void Define(PermissionDefinitionContext context)
    {
        context.Group(CatalogPermissions.GroupName, "Catalog")
            .Add(CatalogPermissions.Brands.View, "View brands")
            .Add(CatalogPermissions.Brands.Create, "Create brands", CatalogPermissions.Brands.View)
            .Add(CatalogPermissions.Brands.Edit, "Edit brands", CatalogPermissions.Brands.View)
            .Add(CatalogPermissions.Brands.Delete, "Delete brands", CatalogPermissions.Brands.View)
            .Add(CatalogPermissions.Brands.Export, "Export brands", CatalogPermissions.Brands.View)
            .Add(CatalogPermissions.Products.View, "View products")
            .Add(CatalogPermissions.Products.Create, "Create products", CatalogPermissions.Products.View)
            .Add(CatalogPermissions.Products.Edit, "Edit products", CatalogPermissions.Products.View)
            .Add(CatalogPermissions.Products.Delete, "Delete products", CatalogPermissions.Products.View)
            .Add(CatalogPermissions.Products.Export, "Export products", CatalogPermissions.Products.View);
        context.Group("Dashboards", "Dashboards")
            .Add(CatalogPermissions.Dashboards.View, "View the dashboard");
    }
}
