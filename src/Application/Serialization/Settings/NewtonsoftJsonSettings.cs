
using CleanArchitectureBase.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace CleanArchitectureBase.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}