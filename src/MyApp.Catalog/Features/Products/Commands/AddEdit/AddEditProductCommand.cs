using Coworkee.Application.Caching;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Commands.AddEdit;

[AiTool("Creates a product of a brand, or changes it when Id is given.")]
public sealed record AddEditProductCommand(Guid? Id, AddEditProductRequest Product) : ICommand<Result<ProductDto>>, IInvalidatesCache
{
    public IReadOnlyList<string> CacheTags => [DashboardCache.Tag];
}
