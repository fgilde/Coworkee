using System.Globalization;
using System.IO;
using System.Linq;

namespace CleanArchitectureBase.Shared.Constants.Localization
{
    public static class LocalizationConstants
    {
        private static LanguageCode[] _languages;
        private static string[] resourceNames;
        public static readonly string DefaultLanguageCode = "en-US";

        public static LanguageCode[] SupportedLanguages
        {
            get
            {
                _languages ??= CultureInfo.GetCultures(CultureTypes.AllCultures).Where(i => !string.IsNullOrWhiteSpace(i.Name) && IsTranslated(i.Name)).Select(LanguageCode.FromCulture).ToArray();
                return _languages;
            }
        }

        private static bool IsTranslated(string code)
        {
            return GetEmbeddedResourceNames().Any(n => n.Contains($".{code}."));
        }

        private static string[] GetEmbeddedResourceNames()
        {
            resourceNames ??= typeof(LocalizationConstants).Assembly.GetManifestResourceNames();
            return resourceNames;
        }
    }
}
