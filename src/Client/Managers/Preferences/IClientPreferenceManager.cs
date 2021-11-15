using System.Threading.Tasks;
using CleanArchitectureBase.Client.Theming;
using CleanArchitectureBase.Shared.Managers;

namespace CleanArchitectureBase.Client.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<ClientTheme> GetCurrentThemeAsync();

        Task SetCurrentThemeName(string themeName);
    }
}