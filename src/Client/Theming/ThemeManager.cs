using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coworkee.Client.JsInterop;
using Coworkee.Client.Managers.Theme;
using Coworkee.SDK;
using Microsoft.JSInterop;

namespace Coworkee.Client.Theming
{
    public class ThemeManager: IThemeManager
    {
        private readonly IApplicationClient _client;
        private readonly IJSRuntime _jsRuntime;

        public ThemeManager(IApplicationClient client, IJSRuntime jsRuntime)
        {
            _client = client;
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> BrowserPrefersDarkMode() => await _jsRuntime.InvokeAsync<bool>(JsNamespace.Get("BrowserHelper", "isDarkMode"));

        public Task<ClientTheme> GetDefaultThemeAsync()
        {
            return Task.FromResult(ClientTheme.DefaultTheme);
            //bool isDark = await BrowserPrefersDarkMode();
            //var available = await ThemesAsync();
            //return isDark && available.ContainsValue(ClientTheme.DarkTheme) 
            //    ? ClientTheme.DarkTheme 
            //    : available.ContainsValue(ClientTheme.DefaultTheme) 
            //        ? ClientTheme.DefaultTheme : available.Count > 0 ? available.FirstOrDefault().Value : ClientTheme.DefaultTheme;
        }

        public ClientTheme CurrentTheme => ClientTheme.LastUsedTheme;

        public Task<Dictionary<string, ClientTheme>> ThemesAsync()
        {
            // TODO: Add from server
            return Task.FromResult(new Dictionary<string, ClientTheme>()
            {
                {nameof(ClientTheme.DefaultTheme), ClientTheme.DefaultTheme},
                {nameof(ClientTheme.LuckyGreen), ClientTheme.LuckyGreen},
                {nameof(ClientTheme.CodeBlue), ClientTheme.CodeBlue}
            });
        }

        public async Task<ClientTheme> GetByNameAsync(string name)
        {
            return (await ThemesAsync()).FirstOrDefault(p => p.Key == name).Value;
        }
    }
}