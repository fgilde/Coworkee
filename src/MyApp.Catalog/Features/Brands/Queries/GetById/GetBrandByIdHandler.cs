using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Brands.Queries.GetById;

internal sealed class GetBrandByIdHandler(CoworkeeDbContext db) : IHandler<GetBrandByIdQuery, Result<BrandDto>>
{
    public async Task<Result<BrandDto>> HandleAsync(GetBrandByIdQuery query, CancellationToken cancellationToken) =>
        await db.Set<Brand>().AsNoTracking().SingleOrDefaultAsync(b => b.Id == query.Id, cancellationToken) is { } brand
            ? brand.ToDto()
            : BrandErrors.NotFound;
}
