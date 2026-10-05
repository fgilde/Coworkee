using Coworkee.Application.Caching;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Brands.Commands.AddEdit;

public sealed record AddEditBrandCommand(Guid? Id, AddEditBrandRequest Brand) : ICommand<Result<BrandDto>>, IInvalidatesCache
{
    public IReadOnlyList<string> CacheTags => [DashboardCache.Tag];
}
