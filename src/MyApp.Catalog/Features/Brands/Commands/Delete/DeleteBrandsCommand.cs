using System.ComponentModel;
using Coworkee.Application.Authorization;
using Coworkee.Application.Caching;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Brands.Commands.Delete;

[Description("Deletes brands by id.")]
[RequiresPermission(CatalogPermissions.Brands.Delete)]
public sealed record DeleteBrandsCommand(IReadOnlyList<Guid> Ids) : ICommand<Result>, IInvalidatesCache
{
    public IReadOnlyList<string> CacheTags => [DashboardCache.Tag];
}
