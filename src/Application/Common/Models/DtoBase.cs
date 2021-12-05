using System.Text.Json.Serialization;

namespace CleanArchitectureBase.Application.Common.Models
{
    public abstract class DtoBase<TId> : IDtoBase<TId>
    {
        public TId Id { get; set; }

        internal bool IsNew => Id == null || Id.Equals(default(TId));

        bool IDtoBase.IsNew => IsNew;
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