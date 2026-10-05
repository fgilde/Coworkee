namespace MyApp.Contracts.Catalog;

public sealed class AddEditProductRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Barcode { get; set; }

    public string? Description { get; set; }

    public string? ImageDataUrl { get; set; }

    public decimal Rate { get; set; }

    public Guid BrandId { get; set; }
}
