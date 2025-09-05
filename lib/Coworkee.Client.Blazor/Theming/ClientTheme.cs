using System;
using lib.Coworkee.Client.Enums;
using MudBlazor;
using MudBlazor.Extensions.Components;
using MudBlazor.Extensions.Core.Enums;
using MudBlazor.Extensions.Helper;

namespace lib.Coworkee.Client.Theming
{

    public class ClientTheme : MudTheme, ICloneable
    {
        public bool ShowFilterInDrawer { get; set; } = true;
        public bool CanPinDrawer { get; set; } = true;
        public bool CanChangeDrawerExpandMode { get; set; }

        public bool Dense { get; set; }
        public bool RowsStriped { get; set; }
        public bool BorderedTables { get; set; }
        
        public DrawerClipMode DrawerClipMode { get; set; } = DrawerClipMode.Always;
        public DrawerVariant DrawerVariant { get; set; } = DrawerVariant.Responsive;

        public TreeViewExpandBehaviour NavMenuExpandMode { get; set; } = TreeViewExpandBehaviour.Default;
        public AppBarTitleBehaviour AppBarTitleBehaviour { get; set; } = AppBarTitleBehaviour.AppNameOnly;
        public MenuTogglePosition MenuTogglePosition { get; set; } = MenuTogglePosition.End;
        public bool ShowUserCardInNavigation { get; set; } = true;
        public bool ShowLogoInAppBar { get; set; } = true;
        public bool ShowLogoInNavMenu { get; set; }
        public bool? PreferDarkMode { get; set; }
        public string Icon { get; set; }
        public string Logo { get; set; }
        public string StyleAppBar { get; set; }
        public MudExCardHoverMode DefaultCardHoverMode { get; set; }


        public ClientTheme Clone()
        {
            return this.CloneTheme();
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}