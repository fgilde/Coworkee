using System.Collections.Generic;
using System.Threading.Tasks;
using Coworkee.Client.Theming;

namespace Coworkee.Client.Managers.Theme
{
    public interface IThemeManager : IManager
    {
        Task<IDictionary<string, ClientTheme>> ThemesAsync();
        Task<ClientTheme> GetByNameAsync(string name);
        Task<bool> BrowserPrefersDarkMode();
        Task<ClientTheme> GetDefaultThemeAsync();
        ClientTheme CurrentTheme { get;  }
    }
}