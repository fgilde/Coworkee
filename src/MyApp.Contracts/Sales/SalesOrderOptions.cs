using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Coworkee.Contracts.Data;

namespace MyApp.Contracts.Sales;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter<SalesOrderOptions>))]
public enum SalesOrderOptions
{
    None = 0,
    [FacetColor("#ef6c00")]
    Express = 1,
    [Display(Name = "Gift wrap")]
    [FacetColor("#ad1457")]
    GiftWrap = 2,
    [FacetColor("#2e7d32")]
    Insured = 4,
    [FacetColor("#6a1b9a")]
    Fragile = 8,
}
