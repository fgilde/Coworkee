using Coworkee.Application.Caching;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Commands.AddEdit;

public sealed record AddEditProductCommand(Guid? Id, AddEditProductRequest Product) : ICommand<Result<ProductDto>>, IInvalidatesCache
{
    public IReadOnlyList<string> CacheTags => [DashboardCache.Tag];
}
