using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;

namespace CleanArchitectureBase.Shared.Constants.Localization
{
    public static class LocalizationConstants
    {
        #region Private fields for caching only

        private static LanguageCode[] _languages;
        private static CultureInfo[] systemCultures;
        private static string[] resourceNames;

        #endregion
        
        public static readonly string DefaultLanguageCode = "en-US"; // Default and pre selected language
        public static readonly string[] DefaultUILanguageCodes = {"en-US", "de-DE", "it-IT", "es-ES", "fr-FR"}; // Default UI Cultures if no admin has specified specific cultures to use

        public static LanguageCode[] SystemCultures =>
            (systemCultures ??= CultureInfo.GetCultures(CultureTypes.AllCultures)).Where(i => !string.IsNullOrWhiteSpace(i.Name)).Select(LanguageCode.FromCulture).ToArray();
        
        public static LanguageCode[] DefaultUILanguages => DefaultUILanguageCodes.Select(LanguageCode.FromCultureCode).ToArray();

        public static LanguageCode[] ExistingTranslations =>
            _languages ??= (systemCultures ??= CultureInfo.GetCultures(CultureTypes.AllCultures)).Where(i => !string.IsNullOrWhiteSpace(i.Name) && IsTranslated(i.Name)).Select(LanguageCode.FromCulture).ToArray();

        public static bool ValidCultureName(string cultureName)
        {
            return Regex.IsMatch(cultureName, @"^[A-Za-z]{1,8}(-[A-Za-z0-9]{1,8})*$")
                   && cultureName.Contains("-");
        }

        public static IDictionary<string, string> GetDefaultLanguageResources()
        {
            var file = GetEmbeddedResourceNames().FirstOrDefault(n => n.Contains($".{DefaultLanguageCode}."));
            if (file == null)
                throw new MissingManifestResourceException("There is no resource found for " + DefaultLanguageCode);
            var serializer = new YamlDotNet.Serialization.Deserializer();
            var stream = typeof(LocalizationConstants).Assembly.GetManifestResourceStream(file);
            using var streamReader = new StreamReader(stream);
            return serializer.Deserialize<Dictionary<string, string>>(streamReader);
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
