using Coworkee.Core.Results;

namespace MyApp.Catalog.Features.Products;

internal static class ProductErrors
{
    public static readonly Error NotFound = Error.NotFound("catalog.product_not_found", "The product does not exist.");

    public static readonly Error UnknownBrand = Error.Validation("BrandId", "The brand does not exist.");
}
