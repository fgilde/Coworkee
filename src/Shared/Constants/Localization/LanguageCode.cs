using System.Globalization;

namespace CleanArchitectureBase.Shared.Constants.Localization
{
    public class LanguageCode
    {
        public LanguageCode()
        { }
        
        public bool? IsRTL { get; set; }
        public string DisplayName { get; set; }
        public string CultureCode { get; set; }
        
        public CultureInfo ToCulture()
        {
            return new CultureInfo(CultureCode);
        }

        public static LanguageCode FromCultureCode(string cultureCode)
        {
            return FromCulture(CultureInfo.GetCultureInfo(cultureCode));
        }

        public static LanguageCode FromCulture(CultureInfo cultureInfo)
        {
            var displayName = cultureInfo.EnglishName;
            return new LanguageCode
            {
                DisplayName = displayName,
                CultureCode = cultureInfo.IetfLanguageTag
            };
        }
    }
}
