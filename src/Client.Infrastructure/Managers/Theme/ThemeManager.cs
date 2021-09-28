using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Infrastructure.Settings;
using CleanArchitectureBase.SDK;
using MudBlazor;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Theme
{
    public class ThemeManager: IThemeManager
    {
        private readonly IBlazorHeroClient _client;

        public ThemeManager(IBlazorHeroClient client)
        {
            _client = client;
        }

        public Task<Dictionary<string, MudTheme>> ThemesAsync()
        {
            // TODO: Add from server
            return Task.FromResult(new Dictionary<string, MudTheme>()
            {
                {nameof(BlazorHeroTheme.DefaultTheme), BlazorHeroTheme.DefaultTheme},
                {nameof(BlazorHeroTheme.DarkTheme), BlazorHeroTheme.DarkTheme}
            });
        }

        public async Task<MudTheme> GetByName(string name)
        {
            return (await ThemesAsync()).FirstOrDefault(p => p.Key == name).Value;
        }
    }
}