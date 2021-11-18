using System.Text.Json.Serialization;

namespace CleanArchitectureBase.Application.Contracts.Enums;

public enum ExportServiceType
{
    [JsonPropertyName(nameof(Excel))] Excel,
    [JsonPropertyName(nameof(Json))] Json
}