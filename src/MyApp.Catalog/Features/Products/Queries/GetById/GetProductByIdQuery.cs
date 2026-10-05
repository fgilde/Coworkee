using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Queries.GetById;

[RequiresPermission(CatalogPermissions.Products.View)]
public sealed record GetProductByIdQuery(Guid Id) : IQuery<Result<ProductDto>>;
