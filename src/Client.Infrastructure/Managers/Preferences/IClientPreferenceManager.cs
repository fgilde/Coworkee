using CleanArchitectureBase.Shared.Managers;
using MudBlazor;
using System.Threading.Tasks;

namespace CleanArchitectureBase.Client.Infrastructure.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<MudTheme> GetCurrentThemeAsync();

        Task SetCurrentThemeName(string themeName);
    }
}