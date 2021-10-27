using System.Globalization;

namespace CleanArchitectureBase.Shared.Constants.Localization
{
    public class LanguageCode
    {
        public bool? IsRTL { get; set; }
        public string DisplayName { get; set; }
        public string Code { get; set; }

        public CultureInfo ToCulture()
        {
            return new CultureInfo(Code);
        }

        public static LanguageCode FromCulture(CultureInfo cultureInfo)
        {
            var displayName = cultureInfo.EnglishName;
            return new LanguageCode
            {
                DisplayName = displayName,
                Code = cultureInfo.IetfLanguageTag
            };
        }
    }
}
