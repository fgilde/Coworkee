using Coworkee.Client.Blazor.Localization;
using System.Globalization;
using Coworkee.Client.Blazor.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyApp.Contracts.Catalog;
using MyApp.Web.Client.Api;

namespace MyApp.Web.Client.Pages;

public partial class Dashboard
{
    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    private DashboardDto? _dashboard;
    private List<ChartSeries<double>> _series = [];
    private string[] _labels = [];

    [Inject] private ICatalogApi Api { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    protected override Task OnInitializedAsync() => LoadAsync();

    private async Task LoadAsync()
    {
        await Snackbar.RunAsync(async () => _dashboard = await Api.GetDashboardAsync());
        if (_dashboard is { } dashboard)
        {
            _labels = [.. dashboard.ProductsPerMonth.Select(m => new DateTime(m.Year, m.Month, 1).ToString("MMM yy", CultureInfo.CurrentCulture))];
            _series = [new ChartSeries<double> { Name = "Products", Data = new ChartData<double>([.. dashboard.ProductsPerMonth.Select(m => (double)m.Count)]) }];
        }

        await InvokeAsync(StateHasChanged);
    }
}
