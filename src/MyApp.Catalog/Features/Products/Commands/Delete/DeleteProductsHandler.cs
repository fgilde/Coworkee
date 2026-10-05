using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;

namespace MyApp.Catalog.Features.Products.Commands.Delete;

internal sealed class DeleteProductsHandler(CoworkeeDbContext db) : IHandler<DeleteProductsCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteProductsCommand command, CancellationToken cancellationToken)
    {
        var products = await db.Set<Product>().Where(p => command.Ids.Contains(p.Id)).ToListAsync(cancellationToken);
        if (products.Count != command.Ids.Distinct().Count())
        {
            return ProductErrors.NotFound;
        }

        db.RemoveRange(products);
        return Result.Success();
    }
}
