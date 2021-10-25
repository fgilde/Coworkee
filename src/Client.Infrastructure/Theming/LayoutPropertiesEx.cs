using CleanArchitectureBase.Client.Infrastructure.Enums;
using MudBlazor;

namespace CleanArchitectureBase.Client.Infrastructure.Theming
{
    public class LayoutPropertiesEx : MudBlazor.LayoutProperties
    {
        public DrawerClipMode DrawerClipMode { get; set; } = DrawerClipMode.Always;
        public DrawerVariant DrawerVariant { get; set; } = DrawerVariant.Responsive;
        public ExpandMode NavMenuExpandMode { get; set; } = ExpandMode.Default;
        public AppBarTitleBehaviour AppBarTitleBehaviour { get; set; } = AppBarTitleBehaviour.AppNameOnly;
        public MenuTogglePosition MenuTogglePosition { get; set; } = MenuTogglePosition.End;
        public bool ShowUserCardInNavigation { get; set; } = true;
        public bool ShowLogoInAppBar { get; set; } = true;
        public bool ShowLogoInNavMenu { get; set; }
    }
}