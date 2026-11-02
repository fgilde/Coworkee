using System.ComponentModel;
using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Queries.GetByBarcode;

/// <summary>Has no route of its own: <see cref="Endpoints.ProductLookupController"/> sends it.</summary>
[Description("Reads the product with a barcode.")]
[RequiresPermission(CatalogPermissions.Products.View)]
public sealed record GetProductByBarcodeQuery(string Barcode) : IQuery<Result<ProductDto>>;
