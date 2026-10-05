namespace MyApp.Contracts.Catalog;

public sealed record ProductDto(Guid Id, string Name, string? Barcode, string? Description, string? ImageDataUrl, decimal Rate, Guid BrandId, BrandDto? Brand);
