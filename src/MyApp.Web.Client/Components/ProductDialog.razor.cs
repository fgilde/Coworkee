using Coworkee.Client.Blazor.Localization;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using MyApp.Contracts.Catalog;
using MyApp.Web.Client.Api;

namespace MyApp.Web.Client.Components;

public partial class ProductDialog
{
    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    private const long MaxImageBytes = 512 * 1024;
    private MudForm _form = null!;
    private IReadOnlyList<BrandDto> _brands = [];

    [CascadingParameter] private IMudDialogInstance Dialog { get; set; } = null!;

    [Inject] private IODataClient OData { get; set; } = null!;

    [Inject] private ICatalogApi Api { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] public string Title { get; set; } = string.Empty;

    [Parameter] public Guid? Id { get; set; }

    [Parameter] public AddEditProductRequest Model { get; set; } = new();

    public static async Task<bool> ShowAsync(IDialogService dialogs, ProductDto? product)
    {
        var parameters = new DialogParameters<ProductDialog>
        {
            { d => d.Title, product is null ? "New product" : "Edit product" },
            { d => d.Id, product?.Id },
            { d => d.Model, product is null ? new AddEditProductRequest() : ToRequest(product) },
        };
        var dialog = await dialogs.ShowAsync<ProductDialog>(null, parameters, new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true });
        return await dialog.Result is { Canceled: false };
    }

    protected override async Task OnInitializedAsync() =>
        _brands = (await OData.QueryAsync<BrandDto>("Brands", new ODataQuery { OrderBy = "Name", Top = 1000, Count = false })).Items;

    private static AddEditProductRequest ToRequest(ProductDto product) => new()
    {
        Name = product.Name,
        Barcode = product.Barcode,
        Description = product.Description,
        ImageDataUrl = product.ImageDataUrl,
        Rate = product.Rate,
        BrandId = product.BrandId,
    };

    private async Task LoadImageAsync(IBrowserFile? file)
    {
        if (file is null)
        {
            return;
        }

        if (file.Size > MaxImageBytes)
        {
            Snackbar.Add(L["The image may have at most 512 KB."], Severity.Warning);
            return;
        }

        await using var stream = file.OpenReadStream(MaxImageBytes);
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        Model.ImageDataUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(memory.ToArray())}";
    }

    private async Task SaveAsync()
    {
        await _form.ValidateAsync();
        if (_form.IsValid && await Snackbar.RunAsync(() => Api.SaveProductAsync(Id, Model), L["Product saved"]))
        {
            Dialog.Close(DialogResult.Ok(true));
        }
    }

    private void Cancel() => Dialog.Cancel();
}
