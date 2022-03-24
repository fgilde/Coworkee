using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using AKSoftware.Localization.MultiLanguages;
using Blazored.LocalStorage;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Client.Managers.Theme;
using CleanArchitectureBase.Client.Theming;
using CleanArchitectureBase.Shared.Constants.Storage;
using CleanArchitectureBase.Shared.Settings;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Nextended.Core;

namespace CleanArchitectureBase.Client.Managers.Preferences
{
    public class ClientPreferenceManager : IClientPreferenceManager
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IStringLocalizer<ClientPreferenceManager> _localizer;
        private readonly IThemeManager _themeManager;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILanguageContainerService _languageService;
        private readonly HttpClient _httpClient;

        public ClientPreferenceManager(
            ILocalStorageService localStorageService,
            IStringLocalizer<ClientPreferenceManager> localizer,
            IThemeManager themeManager, 
            IJSRuntime jsRuntime,
            ILanguageContainerService languageService,
            HttpClient httpClient)
        {
            _localStorageService = localStorageService;
            _localizer = localizer;
            _themeManager = themeManager;
            _jsRuntime = jsRuntime;
            _languageService = languageService;
            _httpClient = httpClient;
        }

        public async Task SetCurrentThemeName(string themeName)
        {
            if (await GetPreference() is ClientPreference preference)
            {
                preference.ThemeName = themeName;
                await SetPreference(preference);
            }
        }

        public async Task<bool> ToggleLayoutDirection()
        {
            if (await GetPreference() is ClientPreference preference)
            {
                preference.IsRTL = !preference.IsRTL;
                await SetPreference(preference);
                return preference.IsRTL;
            }
            return false;
        }

        public async Task<IResult> ChangeLanguageAsync(string languageCode)
        {
            if (await GetPreference() is ClientPreference preference)
            {
                preference.LanguageCode = languageCode;
                var cultureInfo = CultureInfo.GetCultureInfo(languageCode);
                _httpClient.UpdateAcceptLanguage(cultureInfo);
                Check.TryCatch<Exception>(() => _languageService.SetLanguage(cultureInfo));

                await SetPreference(preference);

                return new Result
                {
                    Succeeded = true,
                    Messages = new List<string> { _localizer["Client Language has been changed"] }
                };
            }

            return new Result
            {
                Succeeded = false,
                Messages = new List<string> { _localizer["Failed to get client preferences"] }
            };
        }

        public async Task<ClientTheme> GetCurrentThemeAsync()
        {
            if (await GetPreference() is ClientPreference preference)
                return ClientTheme.LastUsedTheme = await _themeManager.GetByNameAsync(preference.ThemeName) ?? await ClientTheme.BestBrowserMatch(_jsRuntime);
            return await ClientTheme.BestBrowserMatch(_jsRuntime);
        }
        public async Task<bool> IsRTL()
        {
            var preference = await GetPreference() as ClientPreference;
            return preference?.IsRTL ?? false;
        }

        public async Task<IPreference> GetPreference()
        {
            return await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ?? new ClientPreference();
        }

        public async Task SetPreference(IPreference preference)
        { 
            await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);
        }
    }
}