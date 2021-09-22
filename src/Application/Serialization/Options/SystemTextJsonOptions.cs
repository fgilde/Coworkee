using System.Text.Json;
using CleanArchitectureBase.Application.Interfaces.Serialization.Options;

namespace CleanArchitectureBase.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}