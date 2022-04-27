using System.Text.Json.Serialization;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Common.Models
{
    public abstract class HashableDtoBase : IDtoBase<int>
    {
        public virtual bool IsNew => string.IsNullOrEmpty(Id);

        public string Id { get; set; }

        public int GetId() => ((IDtoBase<int>)this).Id;

        int IDtoBase<int>.Id
        {
            get => string.IsNullOrEmpty(Id) ? 0 : Id.MapTo<int>();
            set => Id = value.MapTo<string>();
        }
    }

    public abstract class DtoBase<TId> : IDtoBase<TId>
    {
        public TId Id { get; set; }

        public virtual bool IsNew => Id == null || Id.Equals(default(TId));

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