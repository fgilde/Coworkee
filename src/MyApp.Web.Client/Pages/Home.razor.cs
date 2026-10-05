using Coworkee.Client.Blazor;
using Microsoft.AspNetCore.Components;

namespace MyApp.Web.Client.Pages;

public partial class Home
{
    private const string ProjectUrl = "https://github.com/fgilde/CleanArchitectureBaseBlazor";
    private const string DocumentationUrl = "https://fgilde.github.io/Coworkee/";

    [Inject] private CoworkeeClientOptions Options { get; set; } = null!;
}
