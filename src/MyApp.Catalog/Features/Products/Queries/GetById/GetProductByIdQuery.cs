using System.ComponentModel;
using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Queries.GetById;

[Description("Reads one product with its brand by id.")]
[RequiresPermission(CatalogPermissions.Products.View)]
public sealed record GetProductByIdQuery(Guid Id) : IQuery<Result<ProductDto>>;
