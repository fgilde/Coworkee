using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyApp.Contracts.Sales;

[JsonConverter(typeof(JsonStringEnumConverter<SalesChannel>))]
public enum SalesChannel
{
    [Display(Name = "Web shop")]
    Online,
    Store,
    [Description("By phone")]
    Phone,
    Partner,
}
