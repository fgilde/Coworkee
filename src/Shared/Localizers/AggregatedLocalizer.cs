using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Shared.Localizers
{
    public class AggregatedLocalizer<T> :IStringLocalizer<T>
    {
        private readonly IServiceProvider _provider;

        private IEnumerable<IStringLocalizer<T>> _localizers => _provider.GetServices<IStringLocalizer<T>>().Where(l => l != null && l.GetType() != GetType())
            .OrderBy(l => l.GetType().Name);

        public AggregatedLocalizer(IServiceProvider provider)
        {
            _provider = provider;
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
                return _localizers.Select(l => l[name]).FirstOrDefault(s => !s.ResourceNotFound) ??
                       _localizers.Select(l => l[name]).FirstOrDefault(s => !string.IsNullOrEmpty(s.Value));
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return _localizers.Select(l => l[name, arguments]).FirstOrDefault(s => !s.ResourceNotFound) ??
                       _localizers.Select(l => l[name, arguments]).FirstOrDefault(s => !string.IsNullOrEmpty(s.Value));
            }
        }
    }
}