using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MudBlazor;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Theme
{
    public interface IThemeManager : IManager
    {
        Task<Dictionary<string, MudTheme>> ThemesAsync();
        Task<MudTheme> GetByName(string name);
    }
}