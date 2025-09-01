using System;
using System.Collections.Generic;
using Nextended.Core.Extensions;

namespace Coworkee.Application.Common.Models
{
    public abstract class HashableDtoBase : IDtoBase<int>, IEquatable<HashableDtoBase>
    {
        #region Equality members

        public bool Equals(HashableDtoBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((HashableDtoBase)obj);
        }

        public override int GetHashCode() => (Id != null ? Id.GetHashCode() : 0);

        public static bool operator ==(HashableDtoBase left, HashableDtoBase right) => Equals(left, right);

        public static bool operator !=(HashableDtoBase left, HashableDtoBase right) => !Equals(left, right);

        #endregion

        public virtual bool IsNew => string.IsNullOrEmpty(Id);

        public string Id { get; set; }

        public int GetId() => ((IDtoBase<int>)this).Id;

        int IDtoBase<int>.Id
        {
            get => string.IsNullOrEmpty(Id) ? 0 : Id.MapTo<int>();
            set => Id = value.MapTo<string>();
        }
    }

    public abstract class DtoBase<TId> : IDtoBase<TId>, IEquatable<DtoBase<TId>>
    {
        #region Equality members

        public bool Equals(DtoBase<TId> other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((DtoBase<TId>)obj);
        }

        public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id);

        public static bool operator ==(DtoBase<TId> left, DtoBase<TId> right) => Equals(left, right);

        public static bool operator !=(DtoBase<TId> left, DtoBase<TId> right) => !Equals(left, right);

        #endregion

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