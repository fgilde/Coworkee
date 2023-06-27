using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Client.Theming;
using Coworkee.Shared.Managers;

namespace Coworkee.Client.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<ClientTheme> GetCurrentThemeAsync();
        Task SetActiveSelectedRolesAsync(params UserRoleModel[] roles);
    }
}