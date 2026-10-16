using Coworkee.Contracts.Data;
using Coworkee.Domain;
using MyApp.Contracts.Sales;
using MyApp.Sales.Features.Orders;
using Nextended.Core.Facets;

namespace MyApp.Sales.Domain;

/// <summary>Clients keep all orders in the browser and query them there.</summary>
[Realtime(SalesPermissions.Orders.View)]
[ClientEntity]
public sealed class SalesOrder : AuditedEntity, IMultiTenant
{
    public required string Number { get; set; }

    [ProvideFacet(Label = "Customer", ValuePath = "CustomerId", LabelPath = "Customer.Name", ValueType = typeof(Guid), Order = 4)]
    [FacetBehavior(AllowExclude = true)]
    public Guid CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [ProvideFacet(Label = "Status", Order = 0)]
    [FacetBehavior(View = FacetView.Chips, AllowExclude = true, Switchable = true)]
    public SalesOrderStatus Status { get; set; }

    [ProvideFacet(Label = "Channel", Order = 1)]
    [FacetBehavior(View = FacetView.Buttons, Switchable = true)]
    public SalesChannel Channel { get; set; }

    [ProvideFacet(Label = "Region", Order = 3)]
    [FacetBehavior(Switchable = true)]
    public required string Region { get; set; }

    [ProvideFacet(Label = "Order date", Type = FacetType.DateRange, Order = 5)]
    public DateTimeOffset OrderDate { get; set; }

    [ProvideFacet(Label = "Items", Type = FacetType.Range, Order = 7)]
    public int Items { get; set; }

    [ProvideFacet(Label = "Total", Type = FacetType.Range, Order = 6)]
    [FacetBehavior(Buckets = 16)]
    public decimal Total { get; set; }

    /// <summary>Options that hold together, so "all of" makes sense: express and insured.</summary>
    [ProvideFacet(Label = "Options", Order = 2)]
    [FacetBehavior(View = FacetView.Chips, Combine = FacetCombine.All, Switchable = true)]
    public SalesOrderOptions Options { get; set; }

    [ProvideFacet(Label = "Paid", Order = 8)]
    public bool Paid { get; set; }

    /// <summary>The approval workflow writes its state here (MirrorTo), so the browser can filter, sort and group the orders by it.</summary>
    [ProvideFacet(Label = "Approval", Order = 9)]
    [FacetBehavior(View = FacetView.Chips, Switchable = true)]
    public string ApprovalState { get; set; } = OrderApproval.Initial;

    public Guid TenantId { get; set; }
}
