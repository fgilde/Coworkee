using System.Text.Json.Serialization;

namespace CleanArchitectureBase.Application.Dtos
{
    public abstract class DtoBase<TId> : IDtoBase<TId>
    {
        public TId Id { get; set; }

        [Newtonsoft.Json.JsonIgnore] [JsonIgnore]
        public bool IsNew => Id == null || Id.Equals(default(TId));
    }

    public interface IDtoBase<TId> : IDtoBase
    {
        public TId Id { get; set; }
    }

    public interface IDtoBase
    {
        public bool IsNew { get; }
    }
}