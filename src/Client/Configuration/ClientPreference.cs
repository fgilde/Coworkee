using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Settings;

namespace CleanArchitectureBase.Client.Configuration
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