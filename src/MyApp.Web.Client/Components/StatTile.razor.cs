using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MyApp.Web.Client.Components;

public partial class StatTile
{
    [Parameter, EditorRequired] public string Label { get; set; } = string.Empty;

    [Parameter] public int Value { get; set; }

    [Parameter] public string Icon { get; set; } = Icons.Material.Outlined.Info;

    [Parameter] public Color Color { get; set; } = Color.Primary;
}
