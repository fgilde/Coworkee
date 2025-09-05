using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using AKSoftware.Localization.MultiLanguages;
using Nextended.Core;

namespace lib.Coworkee.Application.Common.Scopes;

public class CultureScope : IDisposable
{
    private readonly ILanguageContainerService _languageContainerService;
    private readonly CultureInfo _culture;
    public CultureScope(CultureInfo culture, ILanguageContainerService languageContainerService)
    {
        _languageContainerService = languageContainerService;
        culture ??= CultureInfo.CurrentCulture;
        _culture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        _languageContainerService?.SetLanguage(culture);
    }

    public CultureScope(string culture, ILanguageContainerService languageContainerService) 
        : this(!string.IsNullOrWhiteSpace(culture) ? Check.TryCatch<CultureInfo, Exception>(() => CultureInfo.GetCultureInfo(culture)) : null, languageContainerService)
    {}

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _culture;
        CultureInfo.CurrentUICulture = _culture;
        _languageContainerService?.SetLanguage(_culture);
    }
}