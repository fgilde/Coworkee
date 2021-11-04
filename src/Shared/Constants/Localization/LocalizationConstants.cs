using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CleanArchitectureBase.Shared.Constants.Localization
{
    public static class LocalizationConstants
    {
        private static LanguageCode[] _languages;
        private static CultureInfo[] systemCultures;
        private static string[] resourceNames;
        public static readonly string DefaultLanguageCode = "en-US";

        public static LanguageCode[] SupportedLanguages
        {
            get
            {
                _languages ??= (systemCultures ??= CultureInfo.GetCultures(CultureTypes.AllCultures)).Where(i => !string.IsNullOrWhiteSpace(i.Name) && IsTranslated(i.Name)).Select(LanguageCode.FromCulture).ToArray();
                return _languages;
            }
        }

        public static bool ValidCultureName(string cultureName)
        {
            return Regex.IsMatch(cultureName, @"^[A-Za-z]{1,8}(-[A-Za-z0-9]{1,8})*$")
                   && cultureName.Contains("-");
            //&& (systemCultures ??= CultureInfo.GetCultures(CultureTypes.AllCultures)).Any(culture => string.Equals(culture.Name, cultureName, StringComparison.CurrentCultureIgnoreCase));
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
