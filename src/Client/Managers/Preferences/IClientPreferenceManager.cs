using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Identity;
using Coworkee.Client.Theming;
using lib.Coworkee.Shared.Managers;

namespace Coworkee.Client.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<ClientTheme> GetCurrentThemeAsync();
        Task SetActiveSelectedRolesAsync(params UserRoleModel[] roles);
    }
}