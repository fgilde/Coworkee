using System.Collections.Generic;
using Blazored.LocalStorage;
using CleanArchitectureBase.Client.Infrastructure.Settings;
using MudBlazor;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Infrastructure.Managers.Theme;
using CleanArchitectureBase.Shared.Constants.Storage;
using CleanArchitectureBase.Shared.Settings;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Preferences
{
    public class ClientPreferenceManager : IClientPreferenceManager
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IStringLocalizer<ClientPreferenceManager> _localizer;
        private readonly IThemeManager _themeManager;

        public ClientPreferenceManager(
            ILocalStorageService localStorageService,
            IStringLocalizer<ClientPreferenceManager> localizer,
            IThemeManager themeManager)
        {
            _localStorageService = localStorageService;
            _localizer = localizer;
            _themeManager = themeManager;
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

        public async Task<MudTheme> GetCurrentThemeAsync()
        {
            if (await GetPreference() is ClientPreference preference)
                return await _themeManager.GetByName(preference.ThemeName) ?? BlazorHeroTheme.DefaultTheme;
            return BlazorHeroTheme.DefaultTheme;
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