using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureBase.Shared.Localizers
{
    public class AggregatedLocalizer<T> :IStringLocalizer<T>
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<AggregatedLocalizer<T>> _logger;

        private IEnumerable<IStringLocalizer<T>> _localizers
        {
            get
            {
                if (_provider == null)
                    return Enumerable.Empty<IStringLocalizer<T>>();
                try
                {
                    return _provider.GetServices<IStringLocalizer<T>>().Where(l => l != null && l.GetType() != GetType())
                        .OrderBy(l => l.GetType().Name);
                }
                catch (Exception)
                {
                    return Enumerable.Empty<IStringLocalizer<T>>();
                }
            }
        }

        public AggregatedLocalizer(IServiceProvider provider, ILogger<AggregatedLocalizer<T>> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var result = new List<List<LocalizedString>>();
            foreach (var localizer in _localizers)
            {
                try
                {
                    // We need to enumerate completely here to ensure we can catch exception for blazor 
                    result.Add(localizer.GetAllStrings(includeParentCultures).ToList());
                }
                catch
                {}
            }
            return result.SelectMany(l => l).Distinct();
        }

        public LocalizedString this[string name]
        {
            get
            {
                var result = _localizers.Select(l => l[name]).FirstOrDefault(s => !s.ResourceNotFound) ??
                       _localizers.Select(l => l[name]).FirstOrDefault(s => !string.IsNullOrEmpty(s.Value));
                //if (result == null || result.ResourceNotFound)
                //    _logger.LogWarning($"Resource Not found. Key: {name}");
                return result;
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var result = _localizers.Select(l => l[name, arguments]).FirstOrDefault(s => !s.ResourceNotFound) ??
                       _localizers.Select(l => l[name, arguments]).FirstOrDefault(s => !string.IsNullOrEmpty(s.Value));
                //if (result == null || result.ResourceNotFound)
                //    _logger.LogWarning($"Resource Not found. Key: {name}");
                return result;
            }
        }
    }
}