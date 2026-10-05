using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;

namespace MyApp.Catalog.Features.Brands.Commands.Delete;

internal sealed class DeleteBrandsHandler(CoworkeeDbContext db) : IHandler<DeleteBrandsCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteBrandsCommand command, CancellationToken cancellationToken)
    {
        var brands = await db.Set<Brand>().Where(b => command.Ids.Contains(b.Id)).ToListAsync(cancellationToken);
        if (brands.Count != command.Ids.Distinct().Count())
        {
            return BrandErrors.NotFound;
        }

        if (await db.Set<Product>().AnyAsync(p => command.Ids.Contains(p.BrandId), cancellationToken))
        {
            return BrandErrors.HasProducts;
        }

        db.RemoveRange(brands);
        return Result.Success();
    }
}
