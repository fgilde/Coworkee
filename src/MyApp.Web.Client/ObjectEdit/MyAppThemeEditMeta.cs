using Coworkee.Client.Blazor.Localization;
using Coworkee.Client.Blazor.Theming;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using MyApp.Web.Client.Theming;

namespace MyApp.Web.Client.ObjectEdit;

/// <summary>
/// How the theme editor shows the properties of <see cref="MyAppTheme"/>; the base configures the library's (logo, custom CSS).
/// This folder holds the forms' configurations: a class implementing IObjectMetaConfiguration&lt;T&gt; applies to every object form of T.
/// </summary>
public class MyAppThemeEditMeta(CoworkeeLocalizer l) : CoworkeeThemeEditMeta<MyAppTheme>(l)
{
    public override async Task ConfigureAsync(ObjectEditMeta<MyAppTheme> meta)
    {
        await base.ConfigureAsync(meta);
        // e.g. meta.Property(t => t.MapStyle).WithLabel(L["Map style"]);
    }
}
