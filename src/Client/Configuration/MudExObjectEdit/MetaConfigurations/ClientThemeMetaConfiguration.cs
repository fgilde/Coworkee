using CleanArchitectureBase.Client.Theming;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using System.Threading.Tasks;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Configuration.MudExObjectEdit.MetaConfigurations;

public class ClientThemeMetaConfiguration : IObjectMetaConfiguration<ClientTheme>
{
    public Task ConfigureAsync(ObjectEditMeta<ClientTheme> meta)
    {
        meta.Property(c => c.LayoutProperties).Children.Recursive(om => om.Children).Ignore();
        meta.Property(c => c.PaletteDark).Children.Recursive(om => om.Children).Ignore();
        meta.Property(c => c.Palette).Children.Recursive(om => om.Children).WrapInMudItem(i => i.xs = 6);
        return Task.CompletedTask;
    }
}