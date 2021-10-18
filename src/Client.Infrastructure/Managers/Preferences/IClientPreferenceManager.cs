using CleanArchitectureBase.Shared.Managers;
using MudBlazor;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Infrastructure.Theming;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<ClientTheme> GetCurrentThemeAsync();

        Task SetCurrentThemeName(string themeName);
    }
}