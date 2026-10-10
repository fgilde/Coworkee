namespace MyApp.Contracts.Sales;

public sealed class AddEditSalesOrderRequest
{
    public Guid CustomerId { get; set; }

    public SalesOrderStatus Status { get; set; }

    public SalesChannel Channel { get; set; }

    public string Region { get; set; } = string.Empty;

    public int Items { get; set; } = 1;

    public decimal Total { get; set; }
}
