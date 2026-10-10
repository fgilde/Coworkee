using System.Text.Json.Serialization;

namespace MyApp.Contracts.Sales;

[JsonConverter(typeof(JsonStringEnumConverter<SalesChannel>))]
public enum SalesChannel
{
    Online,
    Store,
    Phone,
    Partner,
}
