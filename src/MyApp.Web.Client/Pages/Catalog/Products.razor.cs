using Coworkee.Client.Blazor.Localization;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Components.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyApp.Contracts.Catalog;
using MyApp.Web.Client.Api;
using MyApp.Web.Client.Components;

namespace MyApp.Web.Client.Pages.Catalog;

public partial class Products
{
    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    private static readonly string[] SearchFields = [nameof(ProductDto.Name), nameof(ProductDto.Barcode), nameof(ProductDto.Description)];
    private CoworkeeDataTable<ProductDto> _table = null!;

    [Inject] private ICatalogApi Api { get; set; } = null!;

    [Inject] private IDialogService Dialogs { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private Task CreateAsync() => ProductDialog.ShowAsync(Dialogs, null);

    private Task EditAsync(ProductDto product) => ProductDialog.ShowAsync(Dialogs, product);

    private Task DeleteAsync(IReadOnlyCollection<ProductDto> products) => Snackbar.RunAsync(() => Api.DeleteProductsAsync([.. products.Select(p => p.Id)]));
}
