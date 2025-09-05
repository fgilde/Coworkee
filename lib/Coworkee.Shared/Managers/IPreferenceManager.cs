using lib.Coworkee.Shared.Settings;
using System.Threading.Tasks;
using lib.Coworkee.Shared.Wrapper;

namespace lib.Coworkee.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}