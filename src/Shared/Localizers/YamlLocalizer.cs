using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AKSoftware.Localization.MultiLanguages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CleanArchitectureBase.Shared.Localizers
{

    public class YamlLocalizer<T> : IStringLocalizer<T>
    {
        private MethodInfo getValueMethod;
        private FieldInfo keyValuesField;
        private readonly ILanguageContainerService _originalService;
        private readonly ILogger<YamlLocalizer<T>> _logger;

        public YamlLocalizer(ILanguageContainerService originalService, ILogger<YamlLocalizer<T>> logger)
        {
            _originalService = originalService;
            _logger = logger;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            keyValuesField ??= _originalService.Keys.GetType().GetField("keyValues", BindingFlags.Instance | BindingFlags.NonPublic);
            if (keyValuesField != null)
            {
                var keyValues = keyValuesField.GetValue(_originalService.Keys) as JObject;
                foreach (JProperty property in keyValues.Properties())
                {
                    yield return this[property.Name];
                }
            }
        }

        public LocalizedString this[string name] => this[name, null];

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var str = Get(name);
                if (arguments != null && arguments.Any())
                    str.value = string.Format(str.value ?? string.Empty, arguments);
                
                return new LocalizedString(name, str.value);
            }
        }

        private (string value, bool found) Get(string key)
        {
            try
            {
                if (_originalService?.Keys != null)
                {
                    getValueMethod ??= _originalService?.Keys?.GetType().GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic);
                    string res = getValueMethod?.Invoke(_originalService?.Keys, new[] {key})?.ToString();
                    res ??= _originalService?[key];
                    return (res, res != key);
                }
                return (key, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (key, false);
            }
        }
    }
}