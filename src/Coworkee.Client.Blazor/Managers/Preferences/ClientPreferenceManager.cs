using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AKSoftware.Localization.MultiLanguages;
using Blazored.LocalStorage;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Client.Configuration;
using Coworkee.Client.Extensions;
using Coworkee.Client.Managers.Theme;
using Coworkee.Client.Theming;
using Coworkee.Shared.Constants.Storage;
using Coworkee.Shared.Managers;
using Coworkee.Shared.Settings;
using Coworkee.Shared.Wrapper;
using Microsoft.Extensions.Localization;
using MudBlazor.Extensions.Helper;
using Nextended.Core;

namespace Coworkee.Client.Managers.Preferences
{
    public class ClientPreferenceManager : IClientPreferenceManager
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IStringLocalizer<ClientPreferenceManager> _localizer;
        private readonly IThemeManager _themeManager;
        private readonly ILanguageContainerService _languageService;
        private readonly HttpClient _httpClient;

        public ClientPreferenceManager(
            ILocalStorageService localStorageService,
            IStringLocalizer<ClientPreferenceManager> localizer,
            IThemeManager themeManager,
            ILanguageContainerService languageService,
            HttpClient httpClient)
        {
            _localStorageService = localStorageService;
            _localizer = localizer;
            _themeManager = themeManager;
            _languageService = languageService;
            _httpClient = httpClient;
        }
        
        public async Task ToggleLayoutDirection() => await SetPreference(cp => cp.IsRTL = !cp.IsRTL);

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
            if (await GetPreference() is { } preference)
            {
                return ClientThemes.LastUsedTheme =
                    await LoadCustomThemeAsync(preference)
                    ?? await _themeManager.GetByNameAsync(preference.ThemeName) 
                    ?? await _themeManager.GetDefaultThemeAsync();
            }

            return await _themeManager.GetDefaultThemeAsync();
        }
        public async Task<bool> IsRTL() => (await GetPreference())?.IsRTL ?? false;

        async Task<IPreference> IPreferenceManager.GetPreference() => await GetPreference();

        public async Task<ClientPreference> GetPreference() => await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference) ?? new ClientPreference();

        public async Task SetActiveSelectedRolesAsync(params UserRoleModel[] roles)
        {
            if (await GetPreference() is { } preference)
            {
                preference.ActiveSelectedRoles = roles ?? Array.Empty<UserRoleModel>();
                _httpClient.SetActiveRoleIds(roles?.Select(m => m.Id).ToArray());
                await SetPreference(preference);
            }
        }

        public async Task SetPreference(IPreference preference)
        { 
            await _localStorageService.SetItemAsync(StorageConstants.Local.Preference, preference as ClientPreference);
        }

        public async Task SetPreference(Action<ClientPreference> setter)
        {
            if (await GetPreference() is { } preference)
            {
                setter(preference);
                await SetPreference(preference);
            }
        }

        
        public async Task<ClientTheme> LoadCustomThemeAsync(ClientPreference preference)
        {
            preference ??= await GetPreference();
            if (!string.IsNullOrEmpty(preference?.ThemeJson))
            {
                try
                {
                    return MudExThemeHelper.FromJson<ClientTheme>(preference.ThemeJson);
                }
                catch {
                    // ignored
                }
            }
            return null;
        }

        public Task SaveCurrentThemeChangesAsync(ClientTheme theme = null)
        {
            var objThemeJson = theme == null ? string.Empty : theme.AsJson();
            return SetPreference(cp =>
            {
                cp.ThemeJson = objThemeJson;
            });
        }

        public Task SetCurrentThemeAsync(string themeName, ClientTheme theme)
        {
            ClientThemes.LastUsedTheme = theme;
            return SetCurrentThemeNameAsync(themeName);
        }

        private async Task SetCurrentThemeNameAsync(string themeName)
        {
            await SetPreference(cp =>
            {
                cp.ThemeName = themeName;
                cp.ThemeJson = string.Empty; // Delete changes if user selects a new theme
            });
        }
    }
}