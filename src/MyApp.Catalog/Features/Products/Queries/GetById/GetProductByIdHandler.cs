using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Queries.GetById;

internal sealed class GetProductByIdHandler(CoworkeeDbContext db) : IHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken) =>
        await db.Set<Product>().AsNoTracking().Include(p => p.Brand).SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken) is { } product
            ? product.ToDto()
            : ProductErrors.NotFound;
}
