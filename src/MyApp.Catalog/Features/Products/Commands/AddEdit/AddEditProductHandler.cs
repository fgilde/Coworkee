using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Features.Products.Commands.AddEdit;

internal sealed class AddEditProductHandler(CoworkeeDbContext db, IPermissionChecker permissions) : IHandler<AddEditProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> HandleAsync(AddEditProductCommand command, CancellationToken cancellationToken)
    {
        var required = command.Id is null ? CatalogPermissions.Products.Create : CatalogPermissions.Products.Edit;
        if (!await permissions.IsGrantedAsync(required, cancellationToken))
        {
            return Error.Forbidden("catalog.forbidden", "You may not do this.");
        }

        var request = command.Product;
        var brand = await db.Set<Brand>().SingleOrDefaultAsync(b => b.Id == request.BrandId, cancellationToken);
        if (brand is null)
        {
            return ProductErrors.UnknownBrand;
        }

        var product = command.Id is { } id ? await db.Set<Product>().SingleOrDefaultAsync(p => p.Id == id, cancellationToken) : Add(request.Name);
        if (product is null)
        {
            return ProductErrors.NotFound;
        }

        product.Name = request.Name.Trim();
        product.Barcode = request.Barcode;
        product.Description = request.Description;
        product.ImageDataUrl = request.ImageDataUrl;
        product.Rate = request.Rate;
        product.BrandId = brand.Id;
        product.Brand = brand;
        return product.ToDto();
    }

    private Product Add(string name)
    {
        var product = new Product { Name = name };
        db.Add(product);
        return product;
    }
}
