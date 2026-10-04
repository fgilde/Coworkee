using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.AspNetCore;
using Coworkee.AspNetCore.Http;
using Coworkee.Contracts;
using Coworkee.Core.Modularity;
using Coworkee.Core.Results;
using Coworkee.Domain;
using Coworkee.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog;

/// <summary>The example domain: brands and their products, with permissions, audit and live updates from Coworkee.</summary>
public sealed class MyAppCatalogModule : CoworkeeModule, IWebModule
{
    public override void ConfigureServices(ModuleServiceContext context)
    {
        context.Services.AddMessagingFromAssembly(typeof(MyAppCatalogModule).Assembly);
        context.Services.AddSingleton<IModelContributor, CatalogModelContributor>();
        context.Services.AddSingleton<IPermissionDefinitionContributor, CatalogPermissionDefinitions>();
    }

    public void ConfigureApplication(WebApplication app)
    {
        var brands = app.MapGroup("/api/v1/brands").WithTags("Catalog").RequireAuthorization();
        brands.MapGet("/", (IDispatcher d, CancellationToken ct) => d.SendAsync(new GetBrands(), ct).ToHttpResult());
        brands.MapPost("/", (BrandRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new SaveBrand(null, body), ct).ToHttpResult());
        brands.MapPut("/{id:guid}", (Guid id, BrandRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new SaveBrand(id, body), ct).ToHttpResult());
        brands.MapDelete("/{id:guid}", (Guid id, IDispatcher d, CancellationToken ct) => d.SendAsync(new DeleteBrand(id), ct).ToHttpResult());

        var products = app.MapGroup("/api/v1/products").WithTags("Catalog").RequireAuthorization();
        products.MapGet("/", (int? page, int? pageSize, string? search, Guid? brandId, IDispatcher d, CancellationToken ct) =>
            d.SendAsync(new GetProducts(new PageRequest(Math.Max(page ?? 1, 1), Math.Clamp(pageSize ?? 25, 1, 200), search), brandId), ct).ToHttpResult());
        products.MapPost("/", (ProductRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new SaveProduct(null, body), ct).ToHttpResult());
        products.MapPut("/{id:guid}", (Guid id, ProductRequest body, IDispatcher d, CancellationToken ct) => d.SendAsync(new SaveProduct(id, body), ct).ToHttpResult());
        products.MapDelete("/{id:guid}", (Guid id, IDispatcher d, CancellationToken ct) => d.SendAsync(new DeleteProduct(id), ct).ToHttpResult());

        app.MapGet("/api/v1/dashboard", (IDispatcher d, CancellationToken ct) => d.SendAsync(new GetDashboard(), ct).ToHttpResult()).WithTags("Catalog").RequireAuthorization();
    }
}

[Realtime(CatalogPermissions.View)]
public sealed class Brand : AuditedEntity, IMultiTenant
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Tax { get; set; }

    public Guid TenantId { get; set; }
}

[Realtime(CatalogPermissions.View)]
public sealed class Product : AuditedEntity, IMultiTenant
{
    public required string Name { get; set; }

    public string? Barcode { get; set; }

    public string? Description { get; set; }

    public decimal Rate { get; set; }

    public Guid BrandId { get; set; }

    public Guid TenantId { get; set; }
}

internal sealed class CatalogModelContributor : IModelContributor
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(brand =>
        {
            brand.ToTable("Brands", "app");
            brand.Property(b => b.Name).HasMaxLength(200);
            brand.Property(b => b.Description).HasMaxLength(2000);
            brand.Property(b => b.Tax).HasPrecision(9, 4);
            brand.HasIndex(b => new { b.TenantId, b.Name }).IsUnique();
        });

        modelBuilder.Entity<Product>(product =>
        {
            product.ToTable("Products", "app");
            product.Property(p => p.Name).HasMaxLength(200);
            product.Property(p => p.Barcode).HasMaxLength(100);
            product.Property(p => p.Description).HasMaxLength(2000);
            product.Property(p => p.Rate).HasPrecision(18, 4);
            product.HasOne<Brand>().WithMany().HasForeignKey(p => p.BrandId).OnDelete(DeleteBehavior.Restrict);
            product.HasIndex(p => p.BrandId);
        });
    }
}

internal sealed class CatalogPermissionDefinitions : IPermissionDefinitionContributor
{
    public void Define(PermissionDefinitionContext context) =>
        context.Group(CatalogPermissions.GroupName, "Catalog")
            .Add(CatalogPermissions.View, "View brands and products")
            .Add(CatalogPermissions.Manage, "Create, edit and delete brands and products", CatalogPermissions.View);
}

[RequiresPermission(CatalogPermissions.View)]
public sealed record GetBrands : IQuery<Result<IReadOnlyList<BrandDto>>>;

[RequiresPermission(CatalogPermissions.Manage)]
public sealed record SaveBrand(Guid? Id, BrandRequest Request) : ICommand<Result<BrandDto>>;

[RequiresPermission(CatalogPermissions.Manage)]
public sealed record DeleteBrand(Guid Id) : ICommand<Result>;

[RequiresPermission(CatalogPermissions.View)]
public sealed record GetProducts(PageRequest Page, Guid? BrandId) : IQuery<Result<PagedResult<ProductDto>>>;

[RequiresPermission(CatalogPermissions.Manage)]
public sealed record SaveProduct(Guid? Id, ProductRequest Request) : ICommand<Result<ProductDto>>;

[RequiresPermission(CatalogPermissions.Manage)]
public sealed record DeleteProduct(Guid Id) : ICommand<Result>;

[RequiresPermission(CatalogPermissions.View)]
public sealed record GetDashboard : IQuery<Result<DashboardDto>>;

internal sealed class CatalogHandlers(CoworkeeDbContext db, TimeProvider clock)
    : IHandler<GetBrands, Result<IReadOnlyList<BrandDto>>>,
      IHandler<SaveBrand, Result<BrandDto>>,
      IHandler<DeleteBrand, Result>,
      IHandler<GetProducts, Result<PagedResult<ProductDto>>>,
      IHandler<SaveProduct, Result<ProductDto>>,
      IHandler<DeleteProduct, Result>,
      IHandler<GetDashboard, Result<DashboardDto>>
{
    private static readonly Error BrandNotFound = Error.NotFound("catalog.brand_not_found", "The brand does not exist.");
    private static readonly Error ProductNotFound = Error.NotFound("catalog.product_not_found", "The product does not exist.");

    public async Task<Result<IReadOnlyList<BrandDto>>> HandleAsync(GetBrands query, CancellationToken cancellationToken)
    {
        IReadOnlyList<BrandDto> brands = await db.Set<Brand>().AsNoTracking().OrderBy(b => b.Name)
            .Select(b => new BrandDto(b.Id, b.Name, b.Description, b.Tax, db.Set<Product>().Count(p => p.BrandId == b.Id)))
            .ToListAsync(cancellationToken);
        return Result<IReadOnlyList<BrandDto>>.Success(brands);
    }

    public async Task<Result<BrandDto>> HandleAsync(SaveBrand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
        {
            return Error.Validation("Name", "A name of up to 200 characters is required.");
        }

        if (request.Tax is < 0 or > 100)
        {
            return Error.Validation("Tax", "Tax from 0 to 100 percent.");
        }

        if (request.Description is { Length: > 2000 })
        {
            return Error.Validation("Description", "Up to 2000 characters.");
        }

        var name = request.Name.Trim();
        if (await db.Set<Brand>().AnyAsync(b => b.Name == name && b.Id != command.Id, cancellationToken))
        {
            return Error.Conflict("catalog.brand_exists", "A brand with this name exists.");
        }

        Brand brand;
        if (command.Id is { } id)
        {
            if (await db.Set<Brand>().SingleOrDefaultAsync(b => b.Id == id, cancellationToken) is not { } existing)
            {
                return BrandNotFound;
            }

            brand = existing;
        }
        else
        {
            brand = new Brand { Name = name };
            db.Add(brand);
        }

        brand.Name = name;
        brand.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        brand.Tax = request.Tax;
        return new BrandDto(brand.Id, brand.Name, brand.Description, brand.Tax, await db.Set<Product>().CountAsync(p => p.BrandId == brand.Id, cancellationToken));
    }

    public async Task<Result> HandleAsync(DeleteBrand command, CancellationToken cancellationToken)
    {
        if (await db.Set<Brand>().SingleOrDefaultAsync(b => b.Id == command.Id, cancellationToken) is not { } brand)
        {
            return BrandNotFound;
        }

        if (await db.Set<Product>().AnyAsync(p => p.BrandId == brand.Id, cancellationToken))
        {
            return Error.Conflict("catalog.brand_in_use", "Delete or move the brand's products first.");
        }

        db.Remove(brand);
        return Result.Success();
    }

    public async Task<Result<PagedResult<ProductDto>>> HandleAsync(GetProducts query, CancellationToken cancellationToken)
    {
        var products = from product in db.Set<Product>().AsNoTracking()
                       join brand in db.Set<Brand>() on product.BrandId equals brand.Id
                       select new { product, brand.Name };
        if (query.BrandId is { } brandId)
        {
            products = products.Where(p => p.product.BrandId == brandId);
        }

        if (query.Page.Search is { Length: > 0 } search)
        {
            var pattern = "%" + search.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
            products = products.Where(p => EF.Functions.ILike(p.product.Name, pattern) || EF.Functions.ILike(p.product.Barcode ?? string.Empty, pattern) || EF.Functions.ILike(p.Name, pattern));
        }

        var total = await products.CountAsync(cancellationToken);
        var items = await products.OrderBy(p => p.product.Name).Skip((query.Page.Page - 1) * query.Page.PageSize).Take(query.Page.PageSize)
            .Select(p => new ProductDto(p.product.Id, p.product.Name, p.product.Barcode, p.product.Description, p.product.Rate, p.product.BrandId, p.Name))
            .ToListAsync(cancellationToken);
        return new PagedResult<ProductDto>(items, total, query.Page.Page, query.Page.PageSize);
    }

    public async Task<Result<ProductDto>> HandleAsync(SaveProduct command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
        {
            return Error.Validation("Name", "A name of up to 200 characters is required.");
        }

        if (request.Rate is < 0 or >= 100_000_000_000_000m)
        {
            return Error.Validation("Rate", "The rate is from 0 to below 10^14.");
        }

        if (request.Barcode is { Length: > 100 } || request.Description is { Length: > 2000 })
        {
            return Error.Validation("Barcode", "Barcodes up to 100, descriptions up to 2000 characters.");
        }

        if (await db.Set<Brand>().AsNoTracking().SingleOrDefaultAsync(b => b.Id == request.BrandId, cancellationToken) is not { } brand)
        {
            return Error.Validation("BrandId", "Choose an existing brand.");
        }

        Product product;
        if (command.Id is { } id)
        {
            if (await db.Set<Product>().SingleOrDefaultAsync(p => p.Id == id, cancellationToken) is not { } existing)
            {
                return ProductNotFound;
            }

            product = existing;
        }
        else
        {
            product = new Product { Name = string.Empty };
            db.Add(product);
        }

        product.Name = request.Name.Trim();
        product.Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim();
        product.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        product.Rate = request.Rate;
        product.BrandId = brand.Id;
        return new ProductDto(product.Id, product.Name, product.Barcode, product.Description, product.Rate, brand.Id, brand.Name);
    }

    public async Task<Result> HandleAsync(DeleteProduct command, CancellationToken cancellationToken)
    {
        if (await db.Set<Product>().SingleOrDefaultAsync(p => p.Id == command.Id, cancellationToken) is not { } product)
        {
            return ProductNotFound;
        }

        db.Remove(product);
        return Result.Success();
    }

    public async Task<Result<DashboardDto>> HandleAsync(GetDashboard query, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        var start = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(-11);

        // one grouped query instead of one per month
        var created = await db.Set<Product>().AsNoTracking().Where(p => p.CreatedAt >= start)
            .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month }).Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() }).ToListAsync(cancellationToken);
        var months = Enumerable.Range(0, 12).Select(i => start.AddMonths(i))
            .Select(m => new MonthCountDto(m.Year, m.Month, created.FirstOrDefault(c => c.Year == m.Year && c.Month == m.Month)?.Count ?? 0)).ToList();
        return new DashboardDto(await db.Set<Brand>().CountAsync(cancellationToken), await db.Set<Product>().CountAsync(cancellationToken), months);
    }
}
