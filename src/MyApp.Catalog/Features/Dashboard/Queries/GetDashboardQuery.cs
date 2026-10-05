using Coworkee.Application.Authorization;
using Coworkee.Application.Caching;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Dashboard.Queries;

[RequiresPermission(CatalogPermissions.Dashboards.View)]
public sealed record GetDashboardQuery : IQuery<Result<DashboardDto>>, ICachedQuery
{
    public string CacheKey => "dashboard";

    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(5);

    public IReadOnlyList<string> CacheTags => [DashboardCache.Tag];
}
