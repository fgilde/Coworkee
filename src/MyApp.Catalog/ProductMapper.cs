using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;
using Riok.Mapperly.Abstractions;

namespace MyApp.Catalog;

// the generated part lives in Generated/ProductMapper.g.cs; what is declared here replaces the generated declaration
public static partial class ProductMapper
{
    [MapProperty(nameof(Product.Rate), nameof(ProductDto.Rate), Use = nameof(RoundRate))]
    public static partial ProductDto ToDto(this Product product);

    [UserMapping(Default = false)]
    private static decimal RoundRate(decimal rate) => Math.Round(rate, 2);
}
