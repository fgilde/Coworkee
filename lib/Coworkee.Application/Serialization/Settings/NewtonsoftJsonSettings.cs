using lib.Coworkee.Application.Contracts.Serialization.Settings;
using Newtonsoft.Json;

namespace lib.Coworkee.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}