namespace MyApp.Contracts.Catalog;

public static class CatalogPermissions
{
    public const string GroupName = "Catalog";
    public const string View = "Catalog.View";
    public const string Manage = "Catalog.Manage";
}

public sealed record BrandDto(Guid Id, string Name, string? Description, decimal Tax, int ProductCount);

public sealed record BrandRequest(string Name, string? Description = null, decimal Tax = 0);

public sealed record ProductDto(Guid Id, string Name, string? Barcode, string? Description, decimal Rate, Guid BrandId, string BrandName);

public sealed record ProductRequest(string Name, Guid BrandId, string? Barcode = null, string? Description = null, decimal Rate = 0);

/// <summary>Counts for the start page; months are the last twelve, oldest first.</summary>
public sealed record DashboardDto(int Brands, int Products, IReadOnlyList<MonthCountDto> ProductsPerMonth);

public sealed record MonthCountDto(int Year, int Month, int Count);
