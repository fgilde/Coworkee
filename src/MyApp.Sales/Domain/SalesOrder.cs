using Coworkee.Domain;
using MyApp.Contracts.Sales;
using Nextended.Core.Facets;

namespace MyApp.Sales.Domain;

/// <summary>Clients keep all orders in the browser and query them there.</summary>
[Realtime(SalesPermissions.Orders.View)]
[ClientEntity]
public sealed class SalesOrder : AuditedEntity, IMultiTenant
{
    public required string Number { get; set; }

    [ProvideFacet(Label = "Customer", ValuePath = "CustomerId", LabelPath = "Customer.Name", ValueType = typeof(Guid), Order = 3)]
    public Guid CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [ProvideFacet(Label = "Status", Order = 0)]
    public SalesOrderStatus Status { get; set; }

    [ProvideFacet(Label = "Channel", Order = 1)]
    public SalesChannel Channel { get; set; }

    [ProvideFacet(Label = "Region", Order = 2)]
    public required string Region { get; set; }

    [ProvideFacet(Label = "Order date", Type = FacetType.DateRange, Order = 4)]
    public DateTimeOffset OrderDate { get; set; }

    public int Items { get; set; }

    public decimal Total { get; set; }

    public Guid TenantId { get; set; }
}
