using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Coworkee.Infrastructure.Contexts;

namespace Coworkee.Server.Localization
{
    internal class ServerLocalizer<T> : IStringLocalizer<T>
    {
        private readonly IServiceProvider _provider;
        private CultureInfo culture => CultureInfo.CurrentCulture;

        public ServerLocalizer(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var db = _provider.GetService<ApplicationDbContext>();
            return db?.Translations.Where(t => t.CultureCode == culture.Name).Select(t => new LocalizedString(t.Key, t.Value));
        }

        public LocalizedString this[string name] => Get(name);

        public LocalizedString this[string name, params object[] arguments] => Get(name, arguments);


        private LocalizedString Get(string key, params object[] arguments)
        {
            try
            {
                var db = _provider.GetService<ApplicationDbContext>();
                var translation = db.Translations.FirstOrDefault(t => t.CultureCode == culture.Name && t.Key == key);
                string value = translation?.Value ?? key;
                if (arguments != null && arguments.Any())
                    value = string.Format(value, arguments);
                return new LocalizedString(key, value, translation == null);
            }
            catch (Exception)
            {
                return new LocalizedString(key, key, true);
            }
        }
    }
}