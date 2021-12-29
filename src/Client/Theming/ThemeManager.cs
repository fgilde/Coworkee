using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Managers.Theme;
using CleanArchitectureBase.SDK;

namespace CleanArchitectureBase.Client.Theming
{
    public class ThemeManager: IThemeManager
    {
        private readonly IApplicationClient _client;

        public ThemeManager(IApplicationClient client)
        {
            _client = client;
        }

        public Task<Dictionary<string, ClientTheme>> ThemesAsync()
        {
            // TODO: Add from server
            return Task.FromResult(new Dictionary<string, ClientTheme>()
            {
                {nameof(ClientTheme.DefaultTheme), ClientTheme.DefaultTheme},
                {nameof(ClientTheme.LuckyControl), ClientTheme.LuckyControl},
                {nameof(ClientTheme.CodeBlue), ClientTheme.CodeBlue},
                {nameof(ClientTheme.DarkTheme), ClientTheme.DarkTheme}
            });
        }

        public async Task<ClientTheme> GetByNameAsync(string name)
        {
            return (await ThemesAsync()).FirstOrDefault(p => p.Key == name).Value;
        }
    }
}