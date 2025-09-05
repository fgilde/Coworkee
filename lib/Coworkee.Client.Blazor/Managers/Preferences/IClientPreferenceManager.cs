using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Client.Theming;
using lib.Coworkee.Shared.Managers;

namespace lib.Coworkee.Client.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<ClientTheme> GetCurrentThemeAsync();
        Task SetActiveSelectedRolesAsync(params UserRoleModel[] roles);
    }
}