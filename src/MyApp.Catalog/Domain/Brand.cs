using Coworkee.Domain;
using Coworkee.Infrastructure.Persistence;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog.Domain;

[Realtime(CatalogPermissions.Brands.View)]
public sealed class Brand : AuditedEntity, IMultiTenant
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public decimal Tax { get; set; }

    public Guid TenantId { get; set; }
}
