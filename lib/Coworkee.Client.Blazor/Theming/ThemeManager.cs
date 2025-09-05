using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib.Coworkee.Client.JsInterop;
using lib.Coworkee.Client.Managers.Theme;
using lib.Coworkee.SDK;
using Microsoft.JSInterop;

namespace lib.Coworkee.Client.Theming
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
            return Task.FromResult(ClientThemes.DefaultTheme);
        }

        public ClientTheme CurrentTheme => ClientThemes.LastUsedTheme;

        public Task<IDictionary<string, ClientTheme>> ThemesAsync()
        {
            // TODO: Add from server
            return Task.FromResult(ClientThemes.All);
        }

        public async Task<ClientTheme> GetByNameAsync(string name)
        {
            return (await ThemesAsync()).FirstOrDefault(p => p.Key == name).Value;
        }
    }
}