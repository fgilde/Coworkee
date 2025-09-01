using Coworkee.Application.Contracts.Serialization.Settings;
using Newtonsoft.Json;

namespace Coworkee.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}