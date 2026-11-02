using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Queries.GetByBarcode;

internal sealed class GetProductByBarcodeHandler(CoworkeeDbContext db) : IHandler<GetProductByBarcodeQuery, Result<ProductDto>>
{
    // barcodes are not unique: the first by name wins
    public async Task<Result<ProductDto>> HandleAsync(GetProductByBarcodeQuery query, CancellationToken cancellationToken) =>
        await db.Set<Product>().AsNoTracking().Include(p => p.Brand).Where(p => p.Barcode == query.Barcode).OrderBy(p => p.Name)
            .FirstOrDefaultAsync(cancellationToken) is { } product
            ? product.ToDto()
            : ProductErrors.NotFound;
}
