using Coworkee.Domain;
using Coworkee.Infrastructure.Persistence;
using MyApp.Contracts.Catalog;
using Nextended.Core.Facets;

namespace MyApp.Catalog.Domain;

[Realtime(CatalogPermissions.Products.View)]
public sealed class Product : AuditedEntity, IMultiTenant
{
    public required string Name { get; set; }

    public string? Barcode { get; set; }

    public string? Description { get; set; }

    public string? ImageDataUrl { get; set; }

    public decimal Rate { get; set; }

    [ProvideFacet(Label = "Brand", ValuePath = "BrandId", LabelPath = "Brand.Name", ValueType = typeof(Guid))]
    public Guid BrandId { get; set; }

    public Brand? Brand { get; set; }

    public Guid TenantId { get; set; }
}
