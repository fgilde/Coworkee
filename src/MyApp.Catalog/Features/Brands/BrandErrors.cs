using Coworkee.Core.Results;

namespace MyApp.Catalog.Features.Brands;

internal static class BrandErrors
{
    public static readonly Error NotFound = Error.NotFound("catalog.brand_not_found", "The brand does not exist.");

    public static readonly Error NameTaken = Error.Conflict("catalog.brand_name_taken", "A brand with this name already exists.");

    public static readonly Error HasProducts = Error.Conflict("catalog.brand_has_products", "The brand still has products.");
}
