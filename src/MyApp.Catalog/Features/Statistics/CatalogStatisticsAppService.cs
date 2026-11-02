using Coworkee.Application.Authorization;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Statistics;

/// <summary>Registered and mapped by AddCoworkee/UseCoworkee because it implements an <see cref="Coworkee.Contracts.Services.IApplicationService"/>.</summary>
internal sealed class CatalogStatisticsAppService(CoworkeeDbContext db) : ICatalogStatisticsAppService
{
    [RequiresPermission(CatalogPermissions.Products.View)]
    public async Task<CatalogStatisticsDto> GetAsync(CancellationToken cancellationToken = default) => new(
        await db.Set<Brand>().CountAsync(cancellationToken),
        await db.Set<Product>().CountAsync(cancellationToken),
        Math.Round(await db.Set<Product>().AverageAsync(p => (decimal?)p.Rate, cancellationToken) ?? 0, 2));
}
