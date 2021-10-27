using System.Linq;
using CleanArchitectureBase.Shared.Constants.Localization;
using CleanArchitectureBase.Shared.Settings;

namespace CleanArchitectureBase.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.DefaultLanguageCode;

        //TODO - add server preferences
    }
}