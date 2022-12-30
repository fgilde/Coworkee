using Coworkee.Client.Enums;
using MudBlazor;
using System;

namespace Coworkee.Client.Theming
{
    public class LayoutPropertiesEx : MudBlazor.LayoutProperties, ICloneable
    {
        public DrawerClipMode DrawerClipMode { get; set; } = DrawerClipMode.Always;
        public DrawerVariant DrawerVariant { get; set; } = DrawerVariant.Responsive;
        public ExpandMode NavMenuExpandMode { get; set; } = ExpandMode.Default;
        public AppBarTitleBehaviour AppBarTitleBehaviour { get; set; } = AppBarTitleBehaviour.AppNameOnly;
        public MenuTogglePosition MenuTogglePosition { get; set; } = MenuTogglePosition.End;
        public bool ShowUserCardInNavigation { get; set; } = true;
        public bool ShowLogoInAppBar { get; set; } = true;
        public bool ShowLogoInNavMenu { get; set; }
        public object Clone() => MemberwiseClone();
    }
}