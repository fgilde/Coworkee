using Coworkee.Application.Common.Models.Identity;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Settings;

namespace Coworkee.Client.Configuration
{
    public record ClientPreference : IPreference
    {
        public string ThemeName { get; set; }
        public string ThemeJson { get; set; }
        public bool IsRTL { get; set; }
        public bool IsDrawerOpen { get; set; } = true;
        public bool IsDrawerPinned { get; set; } = true;
        public bool DrawerSingleExpand { get; set; }
        public bool? DarkMode { get; set; }
        public string LanguageCode { get; set; } = ApplicationConstants.DefaultLanguageCode;
        public UserRoleModel[] ActiveSelectedRoles { get; set; }
    }
}