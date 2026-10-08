using Coworkee.Client.Blazor.Localization;
using Coworkee.Client.Blazor;
using Microsoft.AspNetCore.Components;

namespace MyApp.Web.Client.Pages;

public partial class Home
{
    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    private const string ProjectUrl = "https://github.com/fgilde/Coworkee";
    private const string DocumentationUrl = "https://fgilde.github.io/CoworkeeLib/";

    [Inject] private CoworkeeClientOptions Options { get; set; } = null!;
}
