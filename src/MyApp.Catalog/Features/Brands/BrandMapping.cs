using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Brands;

internal static class BrandMapping
{
    public static BrandDto ToDto(this Brand brand) => new(brand.Id, brand.Name, brand.Description, brand.Tax);
}
