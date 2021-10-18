using MudBlazor;

namespace CleanArchitectureBase.Client.Infrastructure.Theming
{
    public class LayoutPropertiesEx : MudBlazor.LayoutProperties
    {
        public DrawerClipMode DrawerClipMode { get; set; } = DrawerClipMode.Always;
        public DrawerVariant DrawerVariant { get; set; } = DrawerVariant.Responsive;
    }
}