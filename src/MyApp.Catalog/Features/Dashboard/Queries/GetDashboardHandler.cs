using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Identity.Domain;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Dashboard;
using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Dashboard.Queries;

internal sealed class GetDashboardHandler(CoworkeeDbContext db, TimeProvider clock, IEnumerable<IDashboardCounts> documents)
    : IHandler<GetDashboardQuery, Result<DashboardDto>>
{
    private const int Months = 12;

    public async Task<Result<DashboardDto>> HandleAsync(GetDashboardQuery query, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        var start = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(1 - Months);
        var created = await db.Set<Product>()
            .Where(p => p.CreatedAt >= start)
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync(cancellationToken);
        var perMonth = Enumerable.Range(0, Months)
            .Select(offset => start.AddMonths(offset))
            .Select(month => new MonthCountDto(month.Year, month.Month, created.FirstOrDefault(c => c.Year == month.Year && c.Month == month.Month)?.Count ?? 0))
            .ToList();
        var (documentCount, typeCount) = documents.FirstOrDefault() is { } counts ? await counts.DocumentsAsync(cancellationToken) : (0, 0);
        return new DashboardDto(
            await db.Set<Brand>().CountAsync(cancellationToken),
            await db.Set<Product>().CountAsync(cancellationToken),
            documentCount,
            typeCount,
            await db.Set<User>().CountAsync(cancellationToken),
            await db.Set<Role>().CountAsync(cancellationToken),
            perMonth);
    }
}
