using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Brands.Queries.GetById;

[RequiresPermission(CatalogPermissions.Brands.View)]
public sealed record GetBrandByIdQuery(Guid Id) : IQuery<Result<BrandDto>>;
