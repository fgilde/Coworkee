using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Infrastructure.Theming;
using MudBlazor;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Theme
{
    public interface IThemeManager : IManager
    {
        Task<Dictionary<string, ClientTheme>> ThemesAsync();
        Task<ClientTheme> GetByNameAsync(string name);
    }
}