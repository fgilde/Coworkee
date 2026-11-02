namespace MyApp.Contracts.Catalog;

public static class CatalogPermissions
{
    public const string GroupName = "Catalog";

    public static class Brands
    {
        public const string View = "Catalog.Brands.View";
        public const string Create = "Catalog.Brands.Create";
        public const string Edit = "Catalog.Brands.Edit";
        public const string Delete = "Catalog.Brands.Delete";
    }

    public static class Products
    {
        public const string View = "Catalog.Products.View";
        public const string Create = "Catalog.Products.Create";
        public const string Edit = "Catalog.Products.Edit";
        public const string Delete = "Catalog.Products.Delete";
    }

    public static class Dashboards
    {
        public const string View = "Dashboards.View";
    }
}
