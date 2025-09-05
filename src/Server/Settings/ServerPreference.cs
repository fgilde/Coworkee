using System.Linq;
using lib.Coworkee.Shared.Constants.Localization;
using lib.Coworkee.Shared.Settings;

namespace Coworkee.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.DefaultLanguageCode;

        //TODO - add server preferences
    }
}