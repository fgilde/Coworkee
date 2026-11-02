using System.ComponentModel;
using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Queries.GetByBrand;

/// <summary>Carries its route: UseCoworkee maps it, no MapGet needed. brandId binds from the route to <see cref="BrandId"/>.</summary>
[Description("Lists the products of one brand by name.")]
[RequiresPermission(CatalogPermissions.Products.View)]
[Get("/api/v1/brands/{brandId:guid}/products")]
public sealed record GetBrandProductsQuery(Guid BrandId) : IQuery<Result<IReadOnlyList<ProductDto>>>;
