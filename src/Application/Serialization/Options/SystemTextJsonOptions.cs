using System.Text.Json;
using CleanArchitectureBase.Application.Contracts.Serialization.Options;

namespace CleanArchitectureBase.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}