namespace MyApp.Contracts.Sales;

/// <summary>A row of the OData set "SalesOrders", which clients mirror as a client entity.</summary>
public sealed record SalesOrderDto(
    Guid Id, string Number, Guid CustomerId, CustomerDto? Customer, SalesOrderStatus Status, SalesChannel Channel, string Region, DateTimeOffset OrderDate, int Items, decimal Total);
