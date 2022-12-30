using Coworkee.Application.Common.Models.Identity;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Settings;

namespace Coworkee.Client.Configuration
{
    public record ClientPreference : IPreference
    {
        public string ThemeName { get; set; }
        public bool IsRTL { get; set; }
        public bool IsDrawerOpen { get; set; }
        public string PrimaryColor { get; set; }
        public string LanguageCode { get; set; } = ApplicationConstants.DefaultLanguageCode;
        public UserRoleModel[] ActiveSelectedRoles { get; set; }
    }
}