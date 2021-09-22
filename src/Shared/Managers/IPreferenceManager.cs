using CleanArchitectureBase.Shared.Settings;
using System.Threading.Tasks;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}