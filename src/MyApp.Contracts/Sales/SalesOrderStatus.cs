using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Coworkee.Contracts.Data;

namespace MyApp.Contracts.Sales;

[JsonConverter(typeof(JsonStringEnumConverter<SalesOrderStatus>))]
public enum SalesOrderStatus
{
    [FacetColor("#0288d1")]
    Open,
    [FacetColor("#1565c0")]
    Confirmed,
    [Display(Name = "On the way")]
    [FacetColor("#00838f")]
    Shipped,
    [FacetColor("#2e7d32")]
    Delivered,
    [FacetColor("#9e9e9e")]
    Cancelled,
}
