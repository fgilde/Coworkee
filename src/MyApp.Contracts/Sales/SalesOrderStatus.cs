using System.Text.Json.Serialization;

namespace MyApp.Contracts.Sales;

[JsonConverter(typeof(JsonStringEnumConverter<SalesOrderStatus>))]
public enum SalesOrderStatus
{
    Open,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled,
}
