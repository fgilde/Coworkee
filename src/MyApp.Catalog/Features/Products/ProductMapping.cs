using MyApp.Catalog.Domain;
using MyApp.Catalog.Features.Brands;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products;

internal static class ProductMapping
{
    public static ProductDto ToDto(this Product product) =>
        new(product.Id, product.Name, product.Barcode, product.Description, product.ImageDataUrl, product.Rate, product.BrandId, product.Brand?.ToDto());
}
