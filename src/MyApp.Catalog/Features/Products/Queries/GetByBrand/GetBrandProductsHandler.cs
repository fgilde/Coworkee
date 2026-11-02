using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;
using MyApp.Catalog.Features.Brands;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Queries.GetByBrand;

internal sealed class GetBrandProductsHandler(CoworkeeDbContext db) : IHandler<GetBrandProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    public async Task<Result<IReadOnlyList<ProductDto>>> HandleAsync(GetBrandProductsQuery query, CancellationToken cancellationToken)
    {
        if (!await db.Set<Brand>().AnyAsync(b => b.Id == query.BrandId, cancellationToken))
        {
            return BrandErrors.NotFound;
        }

        var products = await db.Set<Product>().AsNoTracking().Include(p => p.Brand).Where(p => p.BrandId == query.BrandId)
            .OrderBy(p => p.Name).ToListAsync(cancellationToken);
        return products.Select(p => p.ToDto()).ToList();
    }
}
