using Coworkee.Shared.Settings;
using System.Threading.Tasks;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}