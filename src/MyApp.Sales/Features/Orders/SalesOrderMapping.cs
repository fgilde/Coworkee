using MyApp.Contracts.Sales;
using MyApp.Sales.Domain;

namespace MyApp.Sales.Features.Orders;

internal static class SalesOrderMapping
{
    public static SalesOrderDto ToDto(this SalesOrder order) => new(
        order.Id, order.Number, order.CustomerId, order.Customer is { } c ? new CustomerDto(c.Id, c.Name, c.City) : null,
        order.Status, order.Channel, order.Region, order.OrderDate, order.Items, order.Total);

    public static void Apply(this SalesOrder order, AddEditSalesOrderRequest input)
    {
        order.CustomerId = input.CustomerId;
        order.Status = input.Status;
        order.Channel = input.Channel;
        order.Region = input.Region;
        order.Items = input.Items;
        order.Total = input.Total;
    }
}
