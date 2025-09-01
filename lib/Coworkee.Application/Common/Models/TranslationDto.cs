using System;

namespace Coworkee.Application.Common.Models
{
    public class TranslationDto: DtoBase<int>, IEquatable<TranslationDto>
    {
        #region Equality members

        public bool Equals(TranslationDto other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Key == other.Key && Value == other.Value && CultureCode == other.CultureCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((TranslationDto) obj);
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Key, Value, CultureCode);

        public static bool operator ==(TranslationDto left, TranslationDto right) => Equals(left, right);

        public static bool operator !=(TranslationDto left, TranslationDto right) => !Equals(left, right);

        #endregion

        public string Key { get; set; }
        public string Value { get; set; }
        public string CultureCode { get; set; }
    }
}