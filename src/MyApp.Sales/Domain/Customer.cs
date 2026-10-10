using Coworkee.Domain;

namespace MyApp.Sales.Domain;

public sealed class Customer : Entity, IMultiTenant
{
    public required string Name { get; set; }

    public required string City { get; set; }

    public Guid TenantId { get; set; }
}
