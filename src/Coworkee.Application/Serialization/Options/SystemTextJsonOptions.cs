using System.Text.Json;
using Coworkee.Application.Contracts.Serialization.Options;

namespace Coworkee.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}