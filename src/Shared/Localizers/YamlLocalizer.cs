using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AKSoftware.Localization.MultiLanguages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureBase.Shared.Localizers
{

    public class YamlLocalizer<T> : IStringLocalizer<T>
    {
        private MethodInfo methodInfo;
        private readonly ILanguageContainerService _originalService;
        private readonly ILogger<YamlLocalizer<T>> _logger;

        public YamlLocalizer(ILanguageContainerService originalService, ILogger<YamlLocalizer<T>> logger)
        {
            _originalService = originalService;
            _logger = logger;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            yield break;
        }

        public LocalizedString this[string name] => this[name, null];

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var str = Get(name);
                if (!str.found)
                {
                    
                }
                if (arguments != null && arguments.Any())
                {
                    str.value = string.Format(str.value ?? string.Empty, arguments);
                }
                return new LocalizedString(name, str.value);
            }
        }

        private (string value, bool found) Get(string key)
        {
            try
            {
                //string res = _originalService[key];
                methodInfo ??=_originalService.Keys.GetType().GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic);
                string res = methodInfo?.Invoke(_originalService.Keys, new[] {key})?.ToString();
                res ??= _originalService[key];
                return (res, res != key);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (key, false);
            }
        }
    }
}