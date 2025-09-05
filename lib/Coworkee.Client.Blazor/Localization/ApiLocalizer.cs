using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.SDK;
using Microsoft.Extensions.Localization;

namespace lib.Coworkee.Client.Localization
{
    internal static class ApiResources
    {
        
        internal static ConcurrentDictionary<CultureInfo, IList<TranslationDto>> ApiTranslations = new();
        private static Task updateTask;
        private static readonly CancellationTokenSource cts = new();

        internal static Task UpdateEntries(IApplicationClient api, CultureInfo culture, bool force)
        {
            if (force || (updateTask == null && !ApiTranslations.ContainsKey(culture)))
            {
                if (updateTask != null)
                {
                    cts.Cancel();
                    updateTask = null;
                }
                updateTask = Task.Run(async () =>
                {
                    try
                    {
                        var translations = await api.Translations_GetAllAsync(true, null, force, cts.Token);
                        ApiTranslations.AddOrUpdate(culture, _ => translations, (_, l) => translations);
                    }
                    catch
                    {
                        // ignored
                    }
                }, cts.Token).ContinueWith(task => updateTask = null, cts.Token);
            }
            return updateTask;
        }

    }

    /// <summary>
    /// Localizes resources for custom key overrides. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiLocalizer<T> : IStringLocalizer<T>
    {
        private readonly IApplicationClient _api;

        private CultureInfo culture => CultureInfo.CurrentCulture;

        public ApiLocalizer(IApplicationClient api)
        {
            _api = api;
            ApiResources.UpdateEntries(_api, culture, false);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>(); // Not supported
        }

        public LocalizedString this[string name] => Get(name);

        public LocalizedString this[string name, params object[] arguments] => Get(name, arguments);


        private LocalizedString Get(string key, params object[] arguments)
        {
            if (ApiResources.ApiTranslations.TryGetValue(culture, out var translationsForCurrentCulture) && translationsForCurrentCulture?.Any() == true)
            {
                var res = translationsForCurrentCulture.FirstOrDefault(dto => dto.Key == key);
                string value = res?.Value ?? key;
                if (arguments != null && arguments.Any())
                    value = string.Format(value, arguments);
                return new LocalizedString(key, value, res == null);
            }
            return new LocalizedString(key, key, true);
        }
    }
}