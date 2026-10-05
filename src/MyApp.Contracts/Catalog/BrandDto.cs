namespace MyApp.Contracts.Catalog;

public sealed record BrandDto(Guid Id, string Name, string? Description, decimal Tax);
