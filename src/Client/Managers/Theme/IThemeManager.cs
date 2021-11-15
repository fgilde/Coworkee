using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Theming;

namespace CleanArchitectureBase.Client.Managers.Theme
{
    public interface IThemeManager : IManager
    {
        Task<Dictionary<string, ClientTheme>> ThemesAsync();
        Task<ClientTheme> GetByNameAsync(string name);
    }
}