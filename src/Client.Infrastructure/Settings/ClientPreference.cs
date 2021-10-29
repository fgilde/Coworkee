using System.Linq;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Constants.Localization;
using CleanArchitectureBase.Shared.Settings;

namespace CleanArchitectureBase.Client.Infrastructure.Settings
{
    public record ClientPreference : IPreference
    {
        public string ThemeName { get; set; }
        public bool IsRTL { get; set; }
        public bool IsDrawerOpen { get; set; }
        public string PrimaryColor { get; set; }
        public string LanguageCode { get; set; } = ApplicationConstants.DefaultLanguageCode;
    }
}